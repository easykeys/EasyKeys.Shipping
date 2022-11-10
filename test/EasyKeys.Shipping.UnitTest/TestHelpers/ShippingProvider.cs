using Bet.Extensions.Testing.Logging;

using EasyKeys.Shipping.Stamps.Shipment.DependencyInjection;
using EasyKeys.Shipping.Stamps.Tracking.DependencyInjection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EasyKeysShipping.UnitTest.TestHelpers
{
    public static class ShippingProvider
    {
        public static IServiceProvider GetFedExServices(ITestOutputHelper output)
        {
            var services = new ServiceCollection();
            var dic = new Dictionary<string, string>
            {
                { "AzureVault:BaseUrl", "https://easykeys.vault.azure.net/" },
                { "FedExOptions:IsDevelopment", "true" },
            };

            var configBuilder = new ConfigurationBuilder().AddInMemoryCollection(dic);

            configBuilder.AddAzureKeyVault(hostingEnviromentName: "Development", usePrefix: true);
            Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");
            services.AddLogging(x => x.AddXunit(output));

            services.AddSingleton<IConfiguration>(configBuilder.Build());
            services.AddFedExRateProvider();
            services.AddFedExAddressValidation();
            services.AddFedExClient();
            services.AddFedExShipmenProvider();
            services.AddFedExTrackingProvider();

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
    }
}
