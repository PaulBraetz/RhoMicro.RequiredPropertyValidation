namespace RhoMicro.RequiredPropertyValidation.RhoMicro.RequiredPropertyValidation;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

/// <summary>
/// Provides extension methods for integrating required property validation into DI containers.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds required property validation to the service collection.
    /// </summary>
    /// <param name="services">
    /// The service collection to register required property validation to.
    /// </param>
    /// <param name="configure">
    /// An optional callback for configuring registration behavior.
    /// </param>
    /// <returns>
    /// A reference to the service collection, for chaining of further method calls.
    /// </returns>
    public static IServiceCollection AddRequiredPropertyValidation(
        this IServiceCollection services,
        Action<RequiredPropertyValidationConfiguration>? configure = null)
    {
        var config = new RequiredPropertyValidationConfiguration();
        configure?.Invoke(config);

        _ = services.AddSingleton<RequiredPropertyValidator>()
            .AddTransient<IRequiredPropertyValidatorSettings>(sp => sp.GetRequiredService<IOptions<RequiredPropertyValidatorSettings>>().Value)
            .AddOptions<RequiredPropertyValidatorSettings>()
            .BindConfiguration(config.SettingsConfigurationSection)
            .ValidateOnStart();

        return services;
    }
}
