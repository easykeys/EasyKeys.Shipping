using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Bet.Extensions.Testing.Logging;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Xunit.Abstractions;

namespace EasyKeysShipping.UnitTest
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
