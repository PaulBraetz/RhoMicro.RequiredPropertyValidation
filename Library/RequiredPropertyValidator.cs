namespace RhoMicro.RequiredPropertyValidation;

using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

/// <summary>
/// Validates instances against required non-nullable properties that are <see langword="null"/>.
/// </summary>
public sealed class RequiredPropertyValidator(IRequiredPropertyValidatorSettings settings)
{
    private readonly ConcurrentDictionary<Type, Object?> _isValidFunctions = new();
    private readonly ConcurrentDictionary<Type, Object?> _getNullPropertyNamesFunctions = new();

    /// <summary>
    /// Gets a value indicating whether any required non-nullable members of this instance are <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T">The type of instance to validate.</typeparam>
    /// <param name="instance">The instance to validate.</param>
    /// <returns>
    /// <see langword="false"/> if this instance has required non-nullable properties that are <see langword="null"/>; otherwise, <see langword="true"/>.
    /// </returns>
    public Boolean TryValidate<T>(T instance)
#pragma warning disable IDE0075 // Simplify conditional expression
    {
        ArgumentNullException.ThrowIfNull(instance);

        var result = instance is IValidateRequiredProperties<T> validatable
            ? TryValicateCore(validatable)
            : settings.UseReflectionFallback
            ? TryValidateFallback(instance)
            : true;

        return result;
    }
#pragma warning restore IDE0075 // Simplify conditional expression
    private static Boolean TryValicateCore<T>(IValidateRequiredProperties<T> instance) => instance.IsValid;
    private Boolean TryValidateFallback<T>(T instance)
    {
        Boolean result;

#pragma warning disable IDE0045 // Convert to conditional expression
        if(_isValidFunctions.GetOrAdd(instance!.GetType(), CreateIsValidFunction<T>) is Func<T, Boolean> isValidFunction)
        {
            result = isValidFunction.Invoke(instance);
        } else
        {
            result = true;
        }
#pragma warning restore IDE0045 // Convert to conditional expression

        return result;
    }
    private static Func<T, Boolean>? CreateIsValidFunction<T>(Type type)
    {
        var nullabilityContext = new NullabilityInfoContext();
        var parameterExpr = Expression.Parameter(typeof(T));
        var castParamExpr = Expression.Convert(parameterExpr, type);
        var nullConstant = Expression.Constant(null);
        var equalsMethod = typeof(Object).GetMethod("Equals", BindingFlags.Public | BindingFlags.Static)!;

        var nullChecks = type.GetProperties(BindingFlags.Instance)
            .Where(p => p.GetCustomAttribute<RequiredMemberAttribute>() is not null)
            .Where(p => nullabilityContext.Create(p).ReadState == NullabilityState.NotNull)
            .Select(p => Expression.Call(null, equalsMethod, Expression.Property(castParamExpr, p), nullConstant))
            .Aggregate<Expression>(Expression.OrElse);

        if(nullChecks is null)
            return null;

        var lambda = Expression.Lambda<Func<T, Boolean>>(nullChecks, parameterExpr);
        var result = lambda.Compile();

        return result;
    }
    /// <summary>
    /// Gets a value indicating whether any required non-nullable members of this instance are <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T">The type of instance to validate.</typeparam>
    /// <param name="instance">The instance to validate.</param>
    /// <param name="nullProperties">
    /// If any required non-nullable parameters were found to be <see langword="null"/>, this parameter will contain their names.
    /// </param>
    /// <returns>
    /// <see langword="false"/> if this instance has required non-nullable properties that are <see langword="null"/>; otherwise, <see langword="true"/>.
    /// </returns>
    public Boolean TryValidate<T>(T instance, out NullPropertyNameSet nullProperties)
    {
        ArgumentNullException.ThrowIfNull(instance);

        nullProperties = instance is IValidateRequiredProperties<T> validatable
            ? GetNullPropertyNamesCore(validatable)
            : settings.UseReflectionFallback
            ? GetNullPropertyNamesFallback(instance)
            : NullPropertyNameSet.Empty;
        var result = nullProperties.Count == 0;

        return result;
    }
    private static NullPropertyNameSet GetNullPropertyNamesCore<T>(IValidateRequiredProperties<T> instance)
    {
        var nullPropertiesSet = new HashSet<String>(StringComparer.OrdinalIgnoreCase);
        instance.GetNullPropertyNames(nullPropertiesSet);
        var result = new NullPropertyNameSet(nullPropertiesSet);

        return result;
    }
    private NullPropertyNameSet GetNullPropertyNamesFallback<T>(T instance)
    {
        NullPropertyNameSet result;

        if(_getNullPropertyNamesFunctions.GetOrAdd(instance!.GetType(), CreateGetNullPropertyNamesFunction<T>) is Func<T, HashSet<String>> getNullPropertyNamesFunction)
        {
            var nullPropertyNames = getNullPropertyNamesFunction.Invoke(instance);
            result = new NullPropertyNameSet(nullPropertyNames);
        } else
        {
            result = NullPropertyNameSet.Empty;
        }

        return result;
    }
    private static Func<T, HashSet<String>>? CreateGetNullPropertyNamesFunction<T>(Type type)
    {
        var nullabilityContext = new NullabilityInfoContext();
        var parameterExpr = Expression.Parameter(typeof(T));
        var castParamExpr = Expression.Convert(parameterExpr, type);
        var nullConstant = Expression.Constant(null);
        var resultExpr = Expression.Variable(typeof(HashSet<String>));
        var resultInitializationStmt = (Expression)Expression.Assign(resultExpr, Expression.New(typeof(HashSet<String>)));
        var equalsMethod = typeof(Object).GetMethod("Equals", BindingFlags.Public | BindingFlags.Static)!;
        var bodyStmts = type.GetProperties()
            .Where(p => p.GetCustomAttribute<RequiredMemberAttribute>() is not null)
            .Where(p => nullabilityContext.Create(p).ReadState == NullabilityState.NotNull)
            .Select(p =>
            {
                var propExpr = Expression.Property(castParamExpr, p);
                var nullTest = Expression.Call(null, equalsMethod, propExpr, nullConstant);
                var ifTrue = Expression.Call(resultExpr, "Add", null, Expression.Constant(p.Name));
                var conditionStmt = Expression.IfThen(nullTest, ifTrue);

                return conditionStmt;
            }).Prepend(resultInitializationStmt)
            .Append(resultExpr);
        var body = Expression.Block([resultExpr], bodyStmts);

        if(body.Expressions.Count == 2)
            return null;

        var lambda = Expression.Lambda<Func<T, HashSet<String>>>(body, parameterExpr);
        var result = lambda.Compile();

        return result;
    }
    /// <summary>
    /// Validates that required non-nullable members of the instance passed are not null.
    /// </summary>
    /// <typeparam name="T">The type of instance to validate.</typeparam>
    /// <param name="instance">The instance to validate.</param>
    /// <exception cref="RequiredPropertiesValidationException">Thrown if the instance passed contains required non-nullable members that are <see langword="null"/>.</exception>
    public void Validate<T>(T instance)
    {
        ArgumentNullException.ThrowIfNull(instance);

        if(!TryValidate(instance, out var nullProperties))
            throw new RequiredPropertiesValidationException(instance!, nullProperties);
    }
}

file static class Extensions
{
    public static TSource? Aggregate<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource, TSource> func)
    {
        using var e = source.GetEnumerator();

        if(!e.MoveNext())
        {
            return default;
        }

        var result = e.Current;
        while(e.MoveNext())
        {
            result = func(result, e.Current);
        }

        return result;
    }
}