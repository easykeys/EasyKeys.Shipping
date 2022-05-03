using EasyKeys.Shipping.Stamps.Shipment.DependencyInjection;
using EasyKeys.Shipping.Stamps.Tracking.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConsoleServiceCollectionExtensions
{
    public static void ConfigureServices(HostBuilderContext hostBuilder, IServiceCollection services)
    {
        services.AddScoped<IMain, Main>();

        services.AddStampsAddressProvider();

        services.AddStampsRateProvider();

        services.AddStampsShipmentProvider();

        services.AddStampsTrackingProvider();
    }
}
