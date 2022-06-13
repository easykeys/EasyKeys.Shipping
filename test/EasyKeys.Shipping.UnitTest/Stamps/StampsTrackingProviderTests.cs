using Bet.Extensions.Testing.Logging;

using EasyKeys.Shipping.Stamps.Tracking;
using EasyKeys.Shipping.Stamps.Tracking.DependencyInjection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EasyKeysShipping.UnitTest.Stamps
{
    public class StampsTrackingProviderTests
    {
        private readonly ITestOutputHelper _output;
        private readonly IStampsTrackingProvider _trackingProvider;

        public StampsTrackingProviderTests(ITestOutputHelper output)
        {
            _output = output;
            _trackingProvider = GetTrackingProvider();
        }

        [Fact]
        public async Task Track_Shipment_Successfully()
        {
            var result = await _trackingProvider.TrackShipmentAsync("9400111298370020264221", CancellationToken.None);

            Assert.NotNull(result);
        }

        private IStampsTrackingProvider GetTrackingProvider()
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

            services.AddStampsTrackingProvider();

            var sp = services.BuildServiceProvider();
            return sp.GetRequiredService<IStampsTrackingProvider>();
        }
    }
}
