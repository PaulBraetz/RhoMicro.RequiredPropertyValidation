namespace RhoMicro.RequiredMemberValidation;

/// <summary>
/// Contains helper methods for the <c>RhoMicro.RequiredMemberValidation</c> namespace for validating required properties.
/// </summary>
public static class RequiredPropertyValidation
{
    /// <summary>
    /// Gets a value indicating whether any required members of this instance are <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T">The type of instance to validate.</typeparam>
    /// <param name="instance">The instance to validate.</param>
    /// <param name="nullProperties">
    /// If any required parameters were found to be <see langword="null"/>, this parameter will contain their names.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if this instance has required properties that are <see langword="null"/>; otherwise, <see langword="false"/>.
    /// </returns>
    public static Boolean TryValidate<T>(T instance, out NullPropertyNameSet nullProperties)
         where T : IValidateRequiredProperties<T>
    {
        var nullPropertiesSet = new HashSet<String>(StringComparer.OrdinalIgnoreCase);
        instance.GetNullPropertyNames(nullPropertiesSet);
        nullProperties = new NullPropertyNameSet(nullPropertiesSet);

        return nullProperties.Count == 0;
    }
    /// <summary>
    /// Validates that required members of the instance passed are not null.
    /// </summary>
    /// <typeparam name="T">The type of instance to validate.</typeparam>
    /// <param name="instance">The instance to validate.</param>
    /// <exception cref="RequiredPropertiesValidationException">Thrown if the instance passed contains required members that are <see langword="null"/>.</exception>
    public static void Validate<T>(T instance)
        where T : IValidateRequiredProperties<T>
    {
        if(!TryValidate(instance, out var nullProperties))
            throw new RequiredPropertiesValidationException(instance, nullProperties);
    }
}