using EasyKeys.Shipping.DHL.Abstractions.DependencyInjection;
using EasyKeys.Shipping.DHL.Abstractions.Options;
using EasyKeys.Shipping.DHL.Shipment;

using Microsoft.Extensions.DependencyInjection;

namespace EasyKeys.Shipping.DHL.Rates.DependencyInjection;

public static class DHLShipmentServiceCollectionExtensions
{
    /// <summary>
    /// adds the DHLExpressClient to the DI container. required for usage of all dhl express shipping services.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="sectionName"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static IServiceCollection AddDHLExpressShipmentProvider(
        this IServiceCollection services,
        string sectionName = nameof(DHLExpressApiOptions),
        Action<DHLExpressApiOptions, IServiceProvider>? configure = null)
    {
        services.AddDHLExpressClient();

        services.AddTransient<IDHLExpressShipmentProvider, DHLExpressShipmentProvider>();

        return services;
    }
}
