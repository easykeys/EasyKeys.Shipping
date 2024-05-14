using System.Collections;

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
    private readonly IEnumerable<IFedExShipmentProvider> _providers;

    public FedExShipmentProviderTests(ITestOutputHelper output)
    {
        _origin = new Address("11407 Granite St", "Charlotte", "NC", "28273", "US");
        _domestic = new Address("1550 central ave", "Riverside", "CA", "92507", "US");
        _international = new Address("80 Fedex Prkwy", "LONDON", string.Empty, "W1T1JY", "GB", isResidential: false);
        _providers = ServiceProviderInstance.GetFedExServices(output)
            .GetServices<IFedExShipmentProvider>();
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

        foreach (var provider in _providers)
        {
            var label = await provider.CreateShipmentAsync(stype, shipment, shipmentDetails, CancellationToken.None);

            Assert.NotNull(label);
            Assert.False(label.InternalErrors.Any());

            Assert.True(label?.Labels.Any(x => x?.Bytes?.Count > 0));

            // Path to save the PNG file
            var filePath = $"{provider}-{provider.GetType().FullName}-domestic-output.png";

            // Write the byte array to a file
            File.WriteAllBytes(filePath, label.Labels.First().Bytes.First());

            // Write the byte array to a file
            //var result = await _provider.CancelShipmentAsync(label!.Labels.First().TrackingId, CancellationToken.None);
            //Assert.True(result.Succeeded);
        }
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
                ExportLicenseExpirationDate = DateTime.Now.AddDays(1),
            });

        foreach (var provider in _providers)
        {
            var label = await provider.CreateShipmentAsync(stype, shipment, shipmentDetails, CancellationToken.None);
            Assert.NotNull(label);

            Assert.False(label.InternalErrors.Any());
            Assert.True(label?.Labels.Any(x => x?.Bytes?.Count > 0));

            // Path to save the PNG file
            var filePath = $"{provider}-{provider.GetType().FullName}-international-output.png";

            // Write the byte array to a file
            File.WriteAllBytes(filePath, label.Labels.First().Bytes.First());

            // sometimes dev env doesnt send documents
            // Assert.True(label?.Labels.Count > 1);

            // Assert.True(label?.ShippingDocuments.Count > 0);
            //var result = await _provider.CancelShipmentAsync(label.Labels.First().TrackingId, CancellationToken.None);
            //Assert.True(result.Succeeded);
        }
    }
}
