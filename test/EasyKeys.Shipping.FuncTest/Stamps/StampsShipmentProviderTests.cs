using Bet.Extensions.Testing.Logging;

using EasyKeys.Shipping.Stamps.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Rates.Models;
using EasyKeys.Shipping.Stamps.Shipment;
using EasyKeys.Shipping.Stamps.Shipment.DependencyInjection;
using EasyKeys.Shipping.Stamps.Shipment.Models;

using EasyKeysShipping.FuncTest.TestHelpers;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Commodity = EasyKeys.Shipping.Abstractions.Models.Commodity;

namespace EasyKeysShipping.FuncTest.Stamps;

public class StampsShipmentProviderTests
{
    private readonly ITestOutputHelper _output;
    private readonly IStampsShipmentProvider _shipmentProvider;

    public StampsShipmentProviderTests(ITestOutputHelper output)
    {
        _output = output;
        _shipmentProvider = ServiceProviderInstance.GetStampsServices(output).GetRequiredService<IStampsShipmentProvider>();
    }

    [Fact]
    public async Task Process_Domestic_Shipment_Successfully()
    {
        var (sender, recipient) = TestShipments.CreateContactInfo();

        var shipmentDetails = new ShipmentDetails();

        var rateOptions = new RateOptions()
        {
            Sender = sender,
            Recipient = recipient,
            ServiceType = StampsServiceType.Priority
        };

        var labels = await _shipmentProvider.CreateShipmentAsync(
              TestShipments.CreateDomesticShipment(),
              rateOptions,
              shipmentDetails,
              CancellationToken.None);

        Assert.NotNull(labels);
        Assert.NotNull(labels.Labels[0].Bytes[0]);
    }

    [Fact]
    public async Task Process_International_Shipment_Successfully()
    {
        var (sender, recipient) = TestShipments.CreateContactInfo();

        var shipmentDetails = new ShipmentDetails();

        shipmentDetails.CustomsInformation = new CustomsInformation()
        {
            CustomsSigner = "brandon moffett"
        };

        var rateOptions = new RateOptions()
        {
            Sender = sender,
            Recipient = recipient,
            ServiceType = StampsServiceType.FromName("USPMI"),
            DeclaredValue = 1
        };

        shipmentDetails.Commodities.Add(new Commodity()
        {
            Description = "ekjs",
            CountryOfManufacturer = "US",
            PartNumber = "kjsdf",
            Amount = 10m,
            CustomsValue = 1m,
            NumberOfPieces = 1,
            Quantity = 1,
            ExportLicenseNumber = "dsdfs",
            Name = "sdkfsdf",
        });

        var labels = await _shipmentProvider.CreateShipmentAsync(
              TestShipments.CreateInternationalShipment() !,
              rateOptions,
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
