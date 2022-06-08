using Bet.Extensions.Testing.Logging;

using EasyKeys.Shipping.Stamps.Rates;
using EasyKeys.Shipping.Stamps.Rates.Models;

using EasyKeysShipping.UnitTest.TestHelpers;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EasyKeysShipping.UnitTest.Stamps;

public class StampsRateProviderTests
{
    private readonly ITestOutputHelper _output;
    private readonly IStampsRateProvider _rateProvider;

    public StampsRateProviderTests(ITestOutputHelper output)
    {
        _output = output;
        _rateProvider = GetRateProvider();
    }

    [Fact]
    public async Task Return_Shipment_With_Rates_Successfully()
    {
        var result = await _rateProvider.GetDomesticRatesAsync(TestShipments.CreateDomesticShipment(), new RateOptions(), CancellationToken.None);

        Assert.NotNull(result);

        Assert.NotNull(result.Rates);

        Assert.Empty(result.InternalErrors);
    }

    private IStampsRateProvider GetRateProvider()
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
        services.AddStampsRateProvider();
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");
        var sp = services.BuildServiceProvider();
        return sp.GetRequiredService<IStampsRateProvider>();
    }
}
