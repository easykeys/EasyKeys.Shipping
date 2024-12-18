using EasyKeys.Shipping.Amazon.Shipment;
using EasyKeys.Shipping.Amazon.Shipment.Models;

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

        var labels = await _shipmentProvider.CreateSmartShipmentAsync(
              TestShipments.CreateDomesticShipment(),
              shipmentDetails,
              CancellationToken.None);

        Assert.NotNull(labels);
        Assert.NotNull(labels.Labels[0].Bytes[0]);

        // File path where the bytes will be saved
        var filePath = $"{_shipmentProvider}-{_shipmentProvider.GetType().FullName}-domestic-output.png";

        // Write the byte array to a file
        File.WriteAllBytes(filePath, labels.Labels.First().Bytes.First());

        var result = await _shipmentProvider.CancelShipmentAsync
            (labels.Labels.First().TrackingId, CancellationToken.None);
    }
}
