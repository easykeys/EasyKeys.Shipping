using EasyKeys.Shipping.Abstractions;
using EasyKeys.Shipping.FedEx.Abstractions.Options;
using EasyKeys.Shipping.FedEx.AddressValidation;

using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class AddressValidationServiceExtensions
{
    /// <summary>
    /// Adds <see cref="IFedExAddressValidationProvider"/>
    /// and also <see cref="IAddressValidationProvider"/> instance to the DI container.
    /// </summary>
    /// <param name="services">The DI services.</param>
    /// <param name="sectionName">The section name for the configuration. The default is <see cref="FedExOptions"/>.</param>
    /// <param name="configOptions">The configuration for the <see cref="FedExOptions"/>. The default is null.</param>
    /// <returns></returns>
    public static IServiceCollection AddFedExAddressValidation(
        this IServiceCollection services,
        string sectionName = nameof(FedExOptions),
        Action<FedExOptions, IServiceProvider>? configOptions = null)
    {
        services.AddChangeTokenOptions<FedExOptions>(
            sectionName: sectionName,
            configureAction: (options, sp) => configOptions?.Invoke(options, sp));

        services.AddLogging();

        services.AddFedExClient();

        services.TryAddTransient<IFedExAddressValidationProvider, FedExAddressValidationProvider>();

        services.AddTransient<IAddressValidationProvider, FedExAddressValidationProvider>();

        return services;
    }
}
