namespace RhoMicro.RequiredPropertyValidation.RhoMicro.RequiredPropertyValidation;
using System;

/// <summary>
/// Configures the integration of required property validation into DI containers.
/// </summary>
public sealed class RequiredPropertyValidationConfiguration
{
    /// <summary>
    /// Gets or sets the configuration section to bind against the registered 
    /// <see cref="IRequiredPropertyValidatorSettings"/> instance.
    /// </summary>
    public String SettingsConfigurationSection { get; set; } = "RequiredPropertyValidation";
}
