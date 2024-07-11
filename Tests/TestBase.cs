#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace Tests;

using Microsoft.Extensions.DependencyInjection;

using RhoMicro.RequiredPropertyValidation;
using RhoMicro.RequiredPropertyValidation.RhoMicro.RequiredPropertyValidation;

public abstract class TestBase
{
    sealed class Settings : IRequiredPropertyValidatorSettings
    {
        public Boolean UseReflectionFallback { get; init; }
    }
    protected RequiredPropertyValidator GetValidator(Boolean useReflection = false)
    {
        var services = new ServiceCollection();
        _ = services.AddRequiredPropertyValidation()
            .AddSingleton<IRequiredPropertyValidatorSettings>(
                new Settings()
                {
                    UseReflectionFallback = useReflection
                });
        var provider = services.BuildServiceProvider();
        var result = provider.GetRequiredService<RequiredPropertyValidator>();

        return result;
    }
}
