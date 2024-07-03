namespace RhoMicro.RequiredPropertyValidation;

/// <summary>
/// Represents a type whose required properties may be validated to not be <see langword="null"/>.
/// </summary>
/// <typeparam name="T">The type whose required properties are to be validated (CRTP).</typeparam>
public interface IValidateRequiredProperties<T>
{
    /// <summary>
    /// Adds the names of all required properties whose value is <see langword="null"/> to a set.
    /// </summary>
    /// <param name="nullProperties">The set of property names to add names to.</param>
    void GetNullPropertyNames(HashSet<String> nullProperties);
    /// <summary>
    /// Gets a value indicating whether any required members of this instance are <see langword="null"/>.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if this instance has required properties that are <see langword="null"/>; otherwise, <see langword="false"/>.
    /// </returns>
    Boolean IsValid { get; }
}