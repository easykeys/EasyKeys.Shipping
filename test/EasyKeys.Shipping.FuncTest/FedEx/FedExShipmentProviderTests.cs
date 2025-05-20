using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.FedEx.Abstractions.Models;
using EasyKeys.Shipping.FedEx.Rates;
using EasyKeys.Shipping.FedEx.Shipment;
using EasyKeysShipping.FuncTest.TestHelpers;
using Microsoft.Extensions.DependencyInjection;

namespace EasyKeysShipping.FuncTest.FedEx;

public class FedExShipmentProviderTests
{
    private readonly ITestOutputHelper _output;
    private readonly Address _origin;
    private readonly Address _domestic;
    private readonly Address _international;
    private readonly IEnumerable<IFedExShipmentProvider> _providers;

    public FedExShipmentProviderTests(ITestOutputHelper output)
    {
        _origin = new Address("11407 Granite St", "Charlotte", "NC", "28273", "US");
        _domestic = new Address("1550 central ave", "Riverside", "CA", "92507", "US");
        _international = new Address("3601 72 Ave Se", "Calgary", "AB", "T2C 2K3", "CA", isResidential: false);
        _international = new Address("285 Wang Fu Jing Avenue", "BEIJING", "", "100006", "CN", isResidential: false);

        // _international = new Address("80 Fedex Prkwy", "LONDON", string.Empty, "W1T1JY", "GB", isResidential: false);
        //_international = new Address("808 Nelson Street", "Vancouver", "BC", "V6Z 2H1", "CA", isResidential: false);
        //_international = new Address("Road No. 2 Km 59.2", "BARCELONETA", "PR", "00617", "US");

        _providers = ServiceProviderInstance.GetFedExServices(output)
            .GetServices<IFedExShipmentProvider>();
        _output = output;
    }

    [Fact]
    public async Task CreateDelete_Labels_ChargeRecipient_For_Domestic_Shipments_Async()
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

            PaymentType = FedExPaymentType.Recipient,
            CustomsPaymentType = FedExPaymentType.Recipient,
            AccountNumber = "",

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
            Assert.True(label.InternalErrors.Any());
        }
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

            _output.WriteLine($"net charge : {label.Labels.First().TotalCharges.NetCharge}, surcharge : {label.Labels.First().TotalCharges.Surcharges} , basecharge : {label.Labels.First().TotalCharges.BaseCharge}");
            // Path to save the PNG file
            var filePath = $"{provider}-{provider.GetType().FullName}-{stype.ServiceName}-domestic-output.png";

            // Write the byte array to a file
            File.WriteAllBytes(filePath, label.Labels.First().Bytes.First());

            var result = await provider.CancelShipmentAsync(label.Labels.First().TrackingId, CancellationToken.None);
            Assert.True(result.Succeeded);
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
               // ExportLicenseNumber = "26456",
                HarmonizedCode = shipment.DestinationAddress.IsCanadaAddress() ? "8301.70.900000" : "8301.70.000000",
                Quantity = 2,
                QuantityUnits = "EA",
                UnitPrice = 10,
                CustomsValue = 18,
                Amount = 18,
                PartNumber = "167",
                //ExportLicenseExpirationDate = DateTime.Now.AddDays(1),
            });

        foreach (var provider in _providers)
        {
            var label = await provider.CreateShipmentAsync(stype, shipment, shipmentDetails, CancellationToken.None);
            Assert.NotNull(label);

            Assert.False(label.InternalErrors.Any());
            Assert.True(label?.Labels.Any(x => x?.Bytes?.Count > 0));
            _output.WriteLine($"net charge : {label.Labels.First().TotalCharges.NetCharge}, surcharge : {label.Labels.First().TotalCharges.Surcharges} , basecharge : {label.Labels.First().TotalCharges.BaseCharge}");

            // Path to save the PNG file
            var filePath = $"{provider}-{provider.GetType().FullName}-{stype.ServiceName}-international-output.png";

            // Write the byte array to a file
            File.WriteAllBytes(filePath, label.Labels.First().Bytes.First());

            foreach (var doc in label.ShippingDocuments)
            {
                var docFilePath = $"{provider}-{provider.GetType().FullName}-{stype.ServiceName}-international-{doc.DocumentName}-output.{doc.ImageType}";
                // Write the byte array to a file
                File.WriteAllBytes(docFilePath, doc.Bytes.First());
            }

            var result = await provider.CancelShipmentAsync(label.Labels.First().TrackingId, CancellationToken.None);
            Assert.True(result.Succeeded);
        }
    }
}
