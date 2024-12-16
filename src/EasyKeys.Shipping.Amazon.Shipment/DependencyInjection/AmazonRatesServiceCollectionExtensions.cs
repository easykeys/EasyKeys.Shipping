using EasyKeys.Shipping.Amazon.Abstractions.Options;
using Microsoft.Extensions.DependencyInjection;

namespace EasyKeys.Shipping.Amazon.Shipment.DependencyInjection;

public static class AmazonRatesServiceCollectionExtensions
{
    public static IServiceCollection AddRestApiAmazonShipmentProvider(
        this IServiceCollection services,
        string sectionName = nameof(AmazonShippingApiOptions),
        Action<AmazonShippingApiOptions, IServiceProvider>? configOptions = null)
    {
        services.AddLogging();

        services.AddAmazonShippingClient();

        services.AddTransient<IAmazonShippingShipmentProvider, AmazonShippingShipmentProvider>();

        return services;
    }
}
