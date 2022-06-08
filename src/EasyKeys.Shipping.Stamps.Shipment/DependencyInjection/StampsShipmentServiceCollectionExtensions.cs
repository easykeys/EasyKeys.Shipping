using EasyKeys.Shipping.Stamps.Abstractions.Options;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EasyKeys.Shipping.Stamps.Shipment.DependencyInjection;

public static class StampsShipmentServiceCollectionExtensions
{
    /// <summary>
    /// Adds Stamps.om Shipment provider.
    /// </summary>
    /// <param name="services">The DI services.</param>
    /// <param name="sectionName">The name of the configuraiton section.</param>
    /// <param name="configure">The action to configure <see cref="StampsOptions"/>.</param>
    /// <returns></returns>
    public static IServiceCollection AddStampsShipmentProvider(
        this IServiceCollection services,
        string sectionName = nameof(StampsOptions),
        Action<StampsOptions, IServiceProvider>? configure = null)
    {
        services.AddStampsClient(sectionName, configure);

        services.TryAddScoped<IStampsShipmentProvider, StampsShipmentProvider>();

        return services;
    }
}
