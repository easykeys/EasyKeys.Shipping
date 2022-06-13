using EasyKeys.Shipping.FedEx.Abstractions.Options;
using EasyKeys.Shipping.FedEx.Shipment;

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
}
