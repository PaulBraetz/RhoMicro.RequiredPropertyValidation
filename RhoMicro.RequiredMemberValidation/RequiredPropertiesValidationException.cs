namespace RhoMicro.RequiredMemberValidation;

/// <summary>
/// Thrown if an instance of <see cref="IValidateRequiredProperties{T}"/> that contains required members that are <see langword="null"/> is being validated via <see cref="Extensions.ValidateRequiredProperties{T}(T)"/>.
/// </summary>
/// <param name="validatedInstance">
/// The instance validated.
/// </param>
/// <param name="nullProperties">
/// The names of the required properties found to be <see langword="null"/>.
/// </param>
public sealed class RequiredPropertiesValidationException(
    Object validatedInstance,
    NullPropertyNameSet nullProperties)
    : Exception($"Instance {validatedInstance} of type {validatedInstance.GetType()} required properties to nut be null, but some were: {String.Join(", ", nullProperties)}")
{
    /// <summary>
    /// Gets the names of the required properties found to be <see langword="null"/>.
    /// </summary>
    public NullPropertyNameSet NullProperties { get; } = nullProperties;
    /// <summary>
    /// Gets the instance validated.
    /// </summary>
    public Object ValidatedInstance { get; } = validatedInstance;
}
