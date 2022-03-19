using EasyKeys.Shipping.FedEx.Console;
using EasyKeys.Shipping.FedEx.Shipment.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConsoleServiceCollectionExtensions
{
    public static void ConfigureServices(HostBuilderContext hostBuilder, IServiceCollection services)
    {
        services.AddScoped<IMain, Main>();
        services.AddFedExAddressValidation();
        services.AddFedExRateProvider();
        services.AddFedExShipmenProvider();
    }
}
