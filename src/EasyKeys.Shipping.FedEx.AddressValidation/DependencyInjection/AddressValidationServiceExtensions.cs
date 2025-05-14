using EasyKeys.Shipping.Abstractions;
using EasyKeys.Shipping.FedEx.Abstractions.Options;
using EasyKeys.Shipping.FedEx.AddressValidation;

using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class AddressValidationServiceExtensions
{
    /// <summary>
    /// Adds <see cref="IFedExAddressValidationProvider"/> that uses Web Services
    /// and also <see cref="IAddressValidationProvider"/> instance to the DI container. <br/>
    /// Caution: FedEx Web Services Tracking, Address Validation, and Validate Postal Codes WSDLS will be disabled on August 31, 2024. The SOAP based FedEx Web Services is in development containment and has been replaced with FedEx RESTful APIs. To learn more and upgrade your integration from Web Services to FedEx APIs, please visit the FedEx Developer Portal.
    /// </summary>
    /// <param name="services">The DI services.</param>
    /// <param name="sectionName">The section name for the configuration. The default is <see cref="FedExOptions"/>.</param>
    /// <param name="configOptions">The configuration for the <see cref="FedExOptions"/>. The default is null.</param>
    /// <returns></returns>
    public static IServiceCollection AddWebServicesFedExAddressValidationProvider(
        this IServiceCollection services,
        string sectionName = nameof(FedExOptions),
        Action<FedExOptions, IServiceProvider>? configOptions = null)
    {
        services.AddChangeTokenOptions<FedExOptions>(
            sectionName: sectionName,
            configureAction: (options, sp) => configOptions?.Invoke(options, sp));

        services.AddLogging();

        services.AddFedExClient();

        services.TryAddTransient<IFedExAddressValidationProvider, EasyKeys.Shipping.FedEx.AddressValidation.WebServices.Impl.FedExAddressValidationProvider>();

        services.AddTransient<IAddressValidationProvider, EasyKeys.Shipping.FedEx.AddressValidation.WebServices.Impl.FedExAddressValidationProvider>();

        return services;
    }

    /// <summary>
    /// Adds <see cref="IFedExAddressValidationProvider"/>,<see cref="IAddressValidationProvider"/>.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="sectionName"></param>
    /// <param name="configOptions"></param>
    /// <returns></returns>
    public static IServiceCollection AddRestApiFedExAddressValidationProvider(
    this IServiceCollection services,
    string sectionName = nameof(FedExApiOptions),
    Action<FedExApiOptions, IServiceProvider>? configOptions = null)
    {
        services.AddFedExApiClients();

        services.AddTransient<IFedExAddressValidationProvider, EasyKeys.Shipping.FedEx.AddressValidation.RestApi.Impl.FedExAddressValidationProvider>();

        services.AddTransient<IAddressValidationProvider, EasyKeys.Shipping.FedEx.AddressValidation.RestApi.Impl.FedExAddressValidationProvider>();

        return services;
    }
}
