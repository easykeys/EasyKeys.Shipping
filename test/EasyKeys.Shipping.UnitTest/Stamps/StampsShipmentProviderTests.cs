using Bet.Extensions.Testing.Logging;

using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Shipment;
using EasyKeys.Shipping.Stamps.Shipment.DependencyInjection;
using EasyKeys.Shipping.Stamps.Shipment.Models;

using EasyKeysShipping.UnitTest.TestHelpers;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EasyKeysShipping.UnitTest.Stamps
{
    public class StampsShipmentProviderTests
    {
        private readonly ITestOutputHelper _output;
        private readonly IStampsShipmentProvider _shipmentProvider;

        public StampsShipmentProviderTests(ITestOutputHelper output)
        {
            _output = output;
            _shipmentProvider = GetShipmentProvider();
        }

        [Fact]
        public async Task Process_Domestic_Shipment_Successfully()
        {
            var rate = new Rate("USPM", string.Empty, 0m, DateTime.Now) { Name = "USPM" };

            var labels = await _shipmentProvider.CreateShipmentAsync(
                  TestShipments.CreateDomesticShipment(),
                  new ShipmentRequestDetails() { SelectedRate = rate },
                  CancellationToken.None);

            Assert.NotNull(labels);
            Assert.NotNull(labels.Labels[0].Bytes[0]);
        }

        [Fact]
        public async Task Process_International_Shipment_Successfully()
        {
            var rate = new Rate("USPMI", string.Empty, 100m, DateTime.Now);

            var shipmentDetails = new ShipmentRequestDetails() { DeclaredValue = 100m, SelectedRate = rate, CustomsInformation = new CustomsInformation() { CustomsSigner = "brandon moffett" } };

            var labels = await _shipmentProvider.CreateShipmentAsync(
                  TestShipments.CreateInternationalShipment(),
                  shipmentDetails,
                  CancellationToken.None);

            Assert.NotNull(labels);
            Assert.NotNull(labels.Labels[0].Bytes[0]);
        }

        private IStampsShipmentProvider GetShipmentProvider()
        {
            var services = new ServiceCollection();

            var dic = new Dictionary<string, string>
        {
            { "AzureVault:BaseUrl", "https://easykeys.vault.azure.net/" },
        };

            var configBuilder = new ConfigurationBuilder().AddInMemoryCollection(dic);
            configBuilder.AddAzureKeyVault(hostingEnviromentName: "Development", usePrefix: true);

            services.AddLogging(builder => builder.AddXunit(_output));
            services.AddSingleton<IConfiguration>(configBuilder.Build());
            services.AddStampsShipmentProvider();
            Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");
            var sp = services.BuildServiceProvider();
            return sp.GetRequiredService<IStampsShipmentProvider>();
        }
    }
}
