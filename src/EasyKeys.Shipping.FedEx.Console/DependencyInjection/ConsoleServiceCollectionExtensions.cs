using EasyKeys.Shipping.FedEx.Console;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConsoleServiceCollectionExtensions
{
    public static void ConfigureServices(HostBuilderContext hostBuilder, IServiceCollection services)
    {
        services.AddScoped<IMain, Main>();
        services.AddWebServicesFedExAddressValidation();
        services.AddWebServicesFedExRateProvider();
        services.AddFedExShipmenProvider();
        services.AddFedExTrackingProvider();
    }
}
