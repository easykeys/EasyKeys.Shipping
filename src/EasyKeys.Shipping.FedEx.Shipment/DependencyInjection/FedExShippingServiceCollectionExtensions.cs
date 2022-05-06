using EasyKeys.Shipping.FedEx.Abstractions.DependencyInjection;
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
        services.AddChangeTokenOptions<FedExOptions>(sectionName, null, (options, config) => configure?.Invoke(options, config));

        services.AddLogging();

        services.AddFedExClient();

        services.AddTransient<IFedExShipmentProvider, FedExShipmentProvider>();

        return services;
    }
}
