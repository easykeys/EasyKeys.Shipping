using EasyKeys.Shipping.Stamps.AddressValidation.DependencyInjection;
using EasyKeys.Shipping.Stamps.Shipment.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConsoleServiceCollectionExtensions
{
    public static void ConfigureServices(HostBuilderContext hostBuilder, IServiceCollection services)
    {
        services.AddScoped<IMain, Main>();

        services.AddStampsAddressProvider();

        services.AddStampsRateProvider();

        services.AddStampsClient();

        services.AddStampsRateV40Provider();

        services.AddStampsShipmentProvider();
    }
}
