using Bet.Extensions.Testing.Logging;

using EasyKeys.Shipping.Usps.Tracking;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Xunit.Sdk;

namespace EasyKeysShipping.UnitTest
{
    public class UspsTrackingClientTests
    {
        private ITestOutputHelper _output;

        public UspsTrackingClientTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task TestUspsTrackingClientAsync()
        {
            var services = GetServices();
            services.AddUspsTracking();

            var sp = services.BuildServiceProvider();

            var client = sp.GetRequiredService<IUspsTrackingClient>();

            var result = await client.GetTrackInfoAsync("9400111298370588642059", CancellationToken.None);

            Assert.NotNull(result);
        }

        private IServiceCollection GetServices()
        {
            var services = new ServiceCollection();
            var dic = new Dictionary<string, string>
            {
                { "AzureVault:BaseUrl", "https://easykeys.vault.azure.net/" },
                { "UspsOptions:ClientIp", "121.170.33.141" }
            };
            var configBuilder = new ConfigurationBuilder().AddInMemoryCollection(dic);

            configBuilder.AddAzureKeyVault(hostingEnviromentName: "Development", usePrefix: true);
            services.AddLogging(builder => builder.AddXunit(_output));
            services.AddSingleton<IConfiguration>(configBuilder.Build());

            services.AddUspsRateProvider();
            return services;
        }
    }
}
