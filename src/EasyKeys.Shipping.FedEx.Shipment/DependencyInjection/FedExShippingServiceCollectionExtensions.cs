using EasyKeys.Shipping.FedEx.Abstractions.Options;
using EasyKeys.Shipping.FedEx.Shipment;
using EasyKeys.Shipping.FedEx.Shipment.WebServices.Impl;

namespace Microsoft.Extensions.DependencyInjection;

public static class FedExShippingServiceCollectionExtensions
{
    /// <summary>
    /// Adds <see cref="IFedExShipmentProvider"/> implementation.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="sectionName"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static IServiceCollection AddFedExShipmenProvider(
        this IServiceCollection services,
        string sectionName = nameof(FedExOptions),
        Action<FedExOptions, IServiceProvider>? configure = null)
    {
        services.AddLogging();

        services.AddFedExClient(sectionName, configure);

        services.AddTransient<IFedExShipmentProvider, FedExShipmentProvider>();

        return services;
    }

    /// <summary>
    /// Adds <see cref="FedExServiceCollectionExtensions.AddFedExApiClients" />,  <see cref="IFedExShipmentProvider" />.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="sectionName"></param>
    /// <param name="configOptions"></param>
    /// <returns></returns>
    public static IServiceCollection AddRestApiFedExShipmentProvider(
        this IServiceCollection services,
        string sectionName = nameof(FedExApiOptions),
        Action<FedExApiOptions, IServiceProvider>? configOptions = null)
    {
        services.AddLogging();

        services.AddFedExApiClients();

        services.AddTransient<IFedExShipmentProvider, EasyKeys.Shipping.FedEx.Shipment.RestApi.Impl.FedExShipmentProvider>();

        return services;
    }
}
