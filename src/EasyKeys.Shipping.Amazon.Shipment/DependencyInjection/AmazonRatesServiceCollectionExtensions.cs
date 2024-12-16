using EasyKeys.Shipping.Amazon.Abstractions.Options;
using Microsoft.Extensions.DependencyInjection;

namespace EasyKeys.Shipping.Amazon.Shipment.DependencyInjection;

public static class AmazonRatesServiceCollectionExtensions
{
    /// <summary>
    /// Adds the rest API amazon rate provider.Must add AmazonShippingClient to di container.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="sectionName"></param>
    /// <param name="configOptions"></param>
    /// <returns></returns>
    public static IServiceCollection AddRestApiAmazonShipmentProvider(
        this IServiceCollection services,
        string sectionName = nameof(AmazonShippingApiOptions),
        Action<AmazonShippingApiOptions, IServiceProvider>? configOptions = null)
    {
        services.AddTransient<IAmazonShippingShipmentProvider, AmazonShippingShipmentProvider>();

        return services;
    }
}
