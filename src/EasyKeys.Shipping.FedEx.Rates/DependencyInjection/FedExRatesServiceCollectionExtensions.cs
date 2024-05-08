using EasyKeys.Shipping.FedEx.Abstractions.Api.V1.Auth;
using EasyKeys.Shipping.FedEx.Abstractions.Middleware;
using EasyKeys.Shipping.FedEx.Abstractions.Options;
using EasyKeys.Shipping.FedEx.Rates;
using EasyKeys.Shipping.FedEx.Rates.Client.V1;
using EasyKeys.Shipping.FedEx.Rates.Client.V1.Impl;
using EasyKeys.Shipping.FedEx.Rates.WebServices.Impl;

namespace Microsoft.Extensions.DependencyInjection;

public static class FedExRatesServiceCollectionExtensions
{
    /// <summary>
    /// Adds <see cref="IFedExRateProvider"/> implementation.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="sectionName"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static IServiceCollection AddWebServicesFedExRateProvider(
        this IServiceCollection services,
        string sectionName = nameof(FedExOptions),
        Action<FedExOptions, IServiceProvider>? configure = null)
    {
        services.AddLogging();

        services.AddFedExClient(sectionName, configure);

        services.AddTransient<IFedExRateProvider, FedExRateProvider>();

        return services;
    }

    /// <summary>
    /// Adds <see cref="IFedexRatesAndTransitTimesClient"/>, <see cref="IFedExRateProvider"/>
    /// <see cref="IFedExAuthClient"/> and <see cref="AuthRequestMiddleware"/> with configuration options <see cref="FedExApiOptions"/>.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="sectionName"></param>
    /// <param name="configOptions"></param>
    /// <returns></returns>
    public static IServiceCollection AddRestApiFedExRateProvider(
        this IServiceCollection services,
        string sectionName = nameof(FedExApiOptions),
        Action<FedExApiOptions, IServiceProvider>? configOptions = null)
    {
        services.AddChangeTokenOptions<FedExApiOptions>(
            sectionName: sectionName,
            configureAction: (options, sp) => configOptions?.Invoke(options, sp));

        services.AddLogging();

        services.AddFedExAuthApiClient();

        services.AddHttpClient<IFedexRatesAndTransitTimesClient, FedexRatesAndTransitTimesClient>()
            .AddHttpMessageHandler<AuthRequestMiddleware>();

        services.AddTransient<IFedExRateProvider, EasyKeys.Shipping.FedEx.Rates.RestApi.Impl.FedexRateProvider>();

        return services;
    }
}
