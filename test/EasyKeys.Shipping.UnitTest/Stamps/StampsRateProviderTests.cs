
using Bet.Extensions.Testing.Logging;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EasyKeysShipping.UnitTest.Stamps
{
    public class StampsRateProviderTests
    {
        private readonly ITestOutputHelper _output;

        public StampsRateProviderTests(ITestOutputHelper output)
        {
            _output = output;
        }

        private ServiceProvider GetServices()
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
            services.AddFedExRateProvider();

            return services.BuildServiceProvider();
        }
    }
}
