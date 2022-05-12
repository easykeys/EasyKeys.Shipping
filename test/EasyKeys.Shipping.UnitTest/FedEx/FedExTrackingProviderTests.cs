using Bet.Extensions.Testing.Logging;

using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.FedEx.Tracking;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EasyKeysShipping.UnitTest.FedEx;

public class FedExTrackingProviderTests
{
    private readonly ITestOutputHelper _output;
    private readonly IFedExTrackingProvider _trackingProvider;

    public FedExTrackingProviderTests(ITestOutputHelper output)
    {
        _output = output;
        _trackingProvider = GetTrackingProvider();
    }

    [Theory]
    [InlineData("778161615475509")]
    [InlineData("776753740455")]
    [InlineData("272707764259")]
    [InlineData("272712649472")]
    [InlineData("272719656012")]
    public async Task Track_Shipment_Successfully(string trackingId)
    {
        var shipmentLabel = new ShipmentLabel();

        shipmentLabel.Labels.Add(new PackageLabelDetails()
        {
            TrackingId = trackingId
        });

        var result = await _trackingProvider.TrackShipmentAsync(shipmentLabel, CancellationToken.None);

        Assert.NotNull(result.TrackingEvents);
    }

    private IFedExTrackingProvider GetTrackingProvider()
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

        services.AddFedExTrackingProvider();

        var sp = services.BuildServiceProvider();
        return sp.GetRequiredService<IFedExTrackingProvider>();
    }
}
