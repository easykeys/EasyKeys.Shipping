using EasyKeys.Shipping.FedEx.Abstractions.OpenApis.V1.AddressValidation;
using EasyKeys.Shipping.FedEx.Abstractions.OpenApis.V1.Authorization;
using EasyKeys.Shipping.FedEx.Abstractions.OpenApis.V1.RatesAndTransitTimes;
using EasyKeys.Shipping.FedEx.Abstractions.OpenApis.V1.Ship;
using EasyKeys.Shipping.FedEx.Abstractions.OpenApis.V1.TradeDocumentsUpload;
using EasyKeys.Shipping.FedEx.Abstractions.Options;
using EasyKeys.Shipping.FedEx.Abstractions.Services;
using EasyKeys.Shipping.FedEx.Abstractions.Services.Impl;

using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class FedExServiceCollectionExtensions
{
    /// <summary>
    /// Adds FedEx Client Service with configuration options <see cref="FedExOptions"/>.
    /// </summary>
    /// <param name="services">The DI services.</param>
    /// <param name="sectionName">The section name for the options.</param>
    /// <param name="configure">The configuration of options.</param>
    /// <returns></returns>
    public static IServiceCollection AddFedExClient(
        this IServiceCollection services,
        string sectionName = nameof(FedExOptions),
        Action<FedExOptions, IServiceProvider>? configure = null)
    {
        services.AddChangeTokenOptions<FedExOptions>(sectionName, null, (options, config) => configure?.Invoke(options, config));

        services.TryAddScoped<IFedExClientService, FedExClientService>();

        return services;
    }

    /// <summary>
    /// Adds <see cref="AuthorizationApi"/>, <see cref="AddressValidationApi"/>, <see cref="RatesAndTransientTimesApi"/>,<see cref="ShipApi"/>,<see cref="IFedexApiAuthenticatorService"/> with configuration options <see cref="FedExApiOptions"/>.
    /// </summary>
    /// <param name="services">The DI services.</param>
    /// <param name="sectionName">The section name for the options.</param>
    /// <param name="configure">The configuration of options.</param>
    /// <returns></returns>
    public static IServiceCollection AddFedExApiClients(
        this IServiceCollection services,
        string sectionName = nameof(FedExApiOptions),
        Action<FedExApiOptions, IServiceProvider>? configure = null)
    {
        services.AddLogging();

        services.AddChangeTokenOptions<FedExApiOptions>(sectionName, null, (options, config) => configure?.Invoke(options, config));
        services.AddSingleton<IFedexApiAuthenticatorService, FedexApiAuthenticatorService>();

        // add generated api clients
        services.AddHttpClient<AuthorizationApi>();
        services.AddHttpClient<AddressValidationApi>();
        services.AddHttpClient<RatesAndTransientTimesApi>();
        services.AddHttpClient<ShipApi>();
        services.AddHttpClient<TradeDocumentsApi>();

        return services;
    }
}
