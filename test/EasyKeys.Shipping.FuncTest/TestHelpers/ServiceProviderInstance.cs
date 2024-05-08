using Bet.Extensions.Testing.Logging;

using EasyKeys.Shipping.Stamps.Shipment.DependencyInjection;
using EasyKeys.Shipping.Stamps.Tracking.DependencyInjection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EasyKeysShipping.FuncTest.TestHelpers;

public static class ServiceProviderInstance
{
    public static IServiceProvider GetFedExServices(ITestOutputHelper output)
    {
        var services = new ServiceCollection();
        var dic = new Dictionary<string, string>
        {
            { "AzureVault:BaseUrl", "https://easykeysshipping.vault.azure.net/" },
            { "FedExOptions:IsDevelopment", "true" },
            { "FedExApiOptions:IsDevelopment", "true" }
        };

        var configBuilder = new ConfigurationBuilder().AddInMemoryCollection(dic);
        configBuilder.AddAzureKeyVault(hostingEnviromentName: "Development", usePrefix: true);
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");
        services.AddLogging(x => x.AddXunit(output));

        services.AddSingleton<IConfiguration>(configBuilder.Build());
        services.AddFedExDocumentProvider();
        services.AddWebServicesFedExRateProvider();
        services.AddWebServicesFedExAddressValidation();
        services.AddFedExClient();
        services.AddFedExShipmenProvider();
        services.AddFedExTrackingProvider();

        // adress validation apis
        services.AddRestApiFedExAddressValidation();
        services.AddRestApiFedExRateProvider();
        return services.BuildServiceProvider();
    }

    public static IServiceProvider GetStampsServices(ITestOutputHelper output)
    {
        var services = new ServiceCollection();

        var dic = new Dictionary<string, string>
        {
            { "AzureVault:BaseUrl", "https://easykeys.vault.azure.net/" },
            { "StampsOptions:IsDevelopment", "true" },
            { "StampsOptions:UseAuthenticator", "true" },
        };

        var configBuilder = new ConfigurationBuilder().AddInMemoryCollection(dic);
        configBuilder.AddAzureKeyVault(hostingEnviromentName: "Development", usePrefix: true);
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");

        services.AddLogging(x => x.AddXunit(output));
        services.AddSingleton<IConfiguration>(configBuilder.Build());
        services.AddStampsShipmentProvider();
        services.AddStampsClient();
        services.AddStampsAddressProvider();
        services.AddStampsRateProvider();
        services.AddStampsTrackingProvider();
        var sp = services.BuildServiceProvider();
        return services.BuildServiceProvider();
    }

    public static ServiceProvider GetUspsServices(ITestOutputHelper output)
    {
        var services = new ServiceCollection();
        var dic = new Dictionary<string, string?>
        {
            { "AzureVault:BaseUrl", "https://easykeys.vault.azure.net/" },
            { "UspsOptions:ClientIp", "13.107.226.41" },
        };
        var configBuilder = new ConfigurationBuilder().AddInMemoryCollection(dic);

        configBuilder.AddAzureKeyVault(hostingEnviromentName: "Development", usePrefix: true);
        services.AddLogging(builder => builder.AddXunit(output));
        services.AddSingleton<IConfiguration>(configBuilder.Build());

        services.AddUspsRateProvider();
        services.AddUspsTracking();

        return services.BuildServiceProvider();
    }
}
