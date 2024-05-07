using EasyKeys.Shipping.Abstractions;
using EasyKeys.Shipping.FedEx.Abstractions.Api.Middleware;
using EasyKeys.Shipping.FedEx.Abstractions.Options;
using EasyKeys.Shipping.FedEx.AddressValidation.Api.V1;
using EasyKeys.Shipping.FedEx.AddressValidation.Api.V1.Impl;
using EasyKeys.Shipping.FedEx.AddressValidation.WebServices;
using EasyKeys.Shipping.FedEx.AddressValidation.WebServices.Impl;

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

    public static IServiceCollection AddFedExAddressValidationApiV1(
    this IServiceCollection services,
    string sectionName = nameof(FedExApiOptions),
    Action<FedExApiOptions, IServiceProvider>? configOptions = null)
    {
        services.AddChangeTokenOptions<FedExApiOptions>(
            sectionName: sectionName,
            configureAction: (options, sp) => configOptions?.Invoke(options, sp));

        services.AddLogging();

        services.AddFedExAuthApiClient();

        services.AddHttpClient<IFedExAddressValidationApiClient, FedExAddressValidationApiClient>()
            .AddHttpMessageHandler<AuthRequestMiddleware>();

        services.AddTransient<IAddressValidationProvider, FedExAddressValidationApiClient>();

        return services;
    }
}
