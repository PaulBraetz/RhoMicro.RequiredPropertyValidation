namespace RhoMicro.RequiredPropertyValidation;

/// <summary>
/// Provides settings for <see cref="RequiredPropertyValidator"/>s.
/// </summary>
public interface IRequiredPropertyValidatorSettings
{
    /// <summary>
    /// Gets a value indicating whether to use reflection as a fallback to the strongly typed default.
    /// </summary>
    Boolean UseReflectionFallback { get; }
}
