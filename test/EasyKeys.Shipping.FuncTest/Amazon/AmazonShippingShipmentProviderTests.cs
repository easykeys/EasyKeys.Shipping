using EasyKeys.Shipping.Amazon.Shipment;
using EasyKeys.Shipping.Amazon.Shipment.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Rates.Models;

using EasyKeysShipping.FuncTest.TestHelpers;

using Microsoft.Extensions.DependencyInjection;

namespace EasyKeys.Shipping.FuncTest.Amazon;

public class AmazonShippingShipmentProviderTests
{
    private readonly ITestOutputHelper _output;
    private readonly IAmazonShippingShipmentProvider _shipmentProvider;

    public AmazonShippingShipmentProviderTests(ITestOutputHelper output)
    {
        _output = output;
        _shipmentProvider = ServiceProviderInstance.GetAmazonServices(output).GetRequiredService<IAmazonShippingShipmentProvider>();
    }

    [Fact]
    public async Task Process_Domestic_Shipment_Successfully()
    {
        var (sender, recipient) = TestShipments.CreateContactInfo();

        var shipmentDetails = new ShippingDetails();

        shipmentDetails.Sender = sender;
        shipmentDetails.Recipient = recipient;

        var rateOptions = new RateOptions()
        {
            Sender = sender,
            Recipient = recipient,
            ServiceType = StampsServiceType.Priority
        };

        var labels = await _shipmentProvider.CreateSmartShipmentAsync(
              TestShipments.CreateDomesticShipment(),
              shipmentDetails,
              CancellationToken.None);

        Assert.NotNull(labels);
        Assert.NotNull(labels.Labels[0].Bytes[0]);
    }

}
