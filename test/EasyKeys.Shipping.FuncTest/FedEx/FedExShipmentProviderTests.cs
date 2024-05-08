using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.FedEx.Abstractions.Models;
using EasyKeys.Shipping.FedEx.Rates;
using EasyKeys.Shipping.FedEx.Shipment;

using EasyKeysShipping.FuncTest.TestHelpers;

using Microsoft.Extensions.DependencyInjection;

namespace EasyKeysShipping.FuncTest.FedEx;

public class FedExShipmentProviderTests
{
    private readonly Address _origin;
    private readonly Address _domestic;
    private readonly Address _international;
    private readonly IFedExShipmentProvider _provider;

    public FedExShipmentProviderTests(ITestOutputHelper output)
    {
        _origin = new Address("11407 Granite St", "Charlotte", "NC", "28273", "US");
        _domestic = new Address("1550 central ave", "Riverside", "CA", "92507", "US");
        _international = new Address("12 Margaret street Sefton Park", "SEFTON PARK", string.Empty, "5083", "AU");
        _provider = ServiceProviderInstance.GetFedExServices(output)
            .GetRequiredService<IFedExShipmentProvider>();
    }

    [Fact]
    public async Task CreateDelete_Labels_For_Domestic_Shipments_Async()
    {
        var packages = new List<Package>
            {
                // fedex envelope
               FedExRateConfigurator.GetFedExEnvelop(0.05M, 199m),
            };

        var configurator = new FedExRateConfigurator(
               _origin,
               _domestic,
               packages.First(),
               true,
               DateTime.Now);

        var stype = FedExServiceType.FedExStandardOvernight;
        var ptype = FedExPackageType.FedExEnvelope;

        var shipmentOptions = new ShipmentOptions(ptype.Name, DateTime.Now);

        var shipment = new Shipment(_origin, _domestic, packages, shipmentOptions);

        var (sender, recipient) = TestShipments.CreateContactInfo();

        var shipmentDetails = new EasyKeys.Shipping.FedEx.Shipment.Models.ShipmentDetails
        {
            Sender = sender,
            Recipient = recipient,

            TransactionId = "1234",

            PaymentType = FedExPaymentType.Sender,

            RateRequestType = "list",

            LabelOptions = new EasyKeys.Shipping.FedEx.Shipment.Models.LabelOptions()
            {
                LabelFormatType = "COMMON2D",
                ImageType = "PNG",
            }
        };

        var label = await _provider.CreateShipmentAsync(stype, shipment, shipmentDetails, CancellationToken.None);

        Assert.NotNull(label);

        Assert.True(label?.Labels.Any(x => x?.Bytes?.Count > 0));

        var result = await _provider.CancelShipmentAsync(label!.Labels.First().TrackingId, CancellationToken.None);
        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task CreateDelete_Labels_For_International_Shipments_Async()
    {
        var packages = new List<Package>
            {
                // fedex envelope
               FedExRateConfigurator.GetFedExEnvelop(0.05M, insuredValue: 18m),
            };

        var stype = FedExServiceType.FedExInternationalPriority;
        var ptype = FedExPackageType.FedExEnvelope;

        var shipmentOptions = new ShipmentOptions(ptype.Name, DateTime.Now);

        var shipment = new Shipment(_origin, _international, packages, shipmentOptions);

        var (sender, recipient) = TestShipments.CreateContactInfo();

        var shipmentDetails = new EasyKeys.Shipping.FedEx.Shipment.Models.ShipmentDetails
        {
            Sender = sender,
            Recipient = recipient,

            TransactionId = "1234",

            PaymentType = FedExPaymentType.Sender,

            RateRequestType = "list",

            LabelOptions = new EasyKeys.Shipping.FedEx.Shipment.Models.LabelOptions()
            {
                LabelFormatType = "COMMON2D",
                ImageType = "PNG",
                EnableEtd = true
            }
        };

        shipmentDetails.Commodities.Add(
            new Commodity()
            {
                Name = "non-thread rivets",
                NumberOfPieces = 10,
                Description = "description",
                CountryOfManufacturer = "US",
                CIMarksandNumbers = "87123",
                ExportLicenseNumber = "26456",
                HarmonizedCode = "8301.70.000000",
                Quantity = 2,
                QuantityUnits = "EA",
                UnitPrice = 10,
                CustomsValue = 18,
                Amount = 18,
                PartNumber = "167",
            });

        var label = await _provider.CreateShipmentAsync(stype, shipment, shipmentDetails, CancellationToken.None);
        Assert.NotNull(label);

        Assert.True(label?.Labels.Any(x => x?.Bytes?.Count > 0));

        // sometimes dev env doesnt send documents
        // Assert.True(label?.Labels.Count > 1);

        // Assert.True(label?.ShippingDocuments.Count > 0);
        var result = await _provider.CancelShipmentAsync(label.Labels.First().TrackingId, CancellationToken.None);
        Assert.True(result.Succeeded);
    }
}
