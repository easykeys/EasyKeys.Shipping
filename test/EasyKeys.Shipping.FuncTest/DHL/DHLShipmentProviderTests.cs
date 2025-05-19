using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Reflection.Emit;

using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.DHL.Abstractions;
using EasyKeys.Shipping.DHL.Shipment;
using EasyKeys.Shipping.DHL.Shipment.Models;

using EasyKeysShipping.FuncTest.TestHelpers;

using Microsoft.Extensions.DependencyInjection;

namespace EasyKeysShipping.FuncTest.DHL;

public class DHLShipmentProviderTests
{
    private readonly ITestOutputHelper _output;
    private readonly IDHLExpressShipmentProvider _validator;

    public DHLShipmentProviderTests(ITestOutputHelper output)
    {
        _output = output;
        _validator = ServiceProviderInstance.GetDHLServices(output).GetServices<IDHLExpressShipmentProvider>().First();
    }

    [Fact]
    public async Task Get_International_Shipments_Successfully()
    {
        // Get the path to the current executable (the .exe file)
        var exePath = Assembly.GetExecutingAssembly().Location;

        // Get the directory where the executable is located
        var exeDirectory = Path.GetDirectoryName(exePath);

        var logoFileName = "Images/ek_logo_main4 (Custom).gif";
        var logoFilePath = Path.Combine(exeDirectory, logoFileName);
        var logoFileContent = File.ReadAllBytes(logoFilePath);

        var signatureFileName = "Images/signature.png";
        var signatureFilePath = Path.Combine(exeDirectory, signatureFileName);
        var signatureFileContent = File.ReadAllBytes(signatureFilePath);

        var shipment = TestShipments.CreateInternationalShipment();

        var contactInfo = TestShipments.CreateContactInfo();

        var shipmentDetails = new EasyKeys.Shipping.DHL.Shipment.Models.ShippingDetails
        {
            Sender = contactInfo.Item1,
            Recipient = contactInfo.Item2,
            CustomShipmentMessage = "Custom message",
            InvoiceNumber = "234333432343",
            PackageDescription = "Pacakge Description",
            ProductCode = "P",
            LabelDescription = "Label Description",
            Logos = [new Logo
            {
                FileFormat = 1,
                Content = Convert.ToBase64String(logoFileContent)
            }

            ],
            Signature = new Signature
            {
                SignatureName = "Test Signature",
                SignatureTitle = "test Title",
                Content = Convert.ToBase64String(signatureFileContent),
            },
            AddedServices = new Dictionary<string, double?>
            {
                { "II", 10 },
                //{ "SF",null }
                //{ "WF", null }
            },
        };

        shipmentDetails.Commodities.Add(
            new Commodity()
            {
                Weight = 0.5m,
                Name = "key # 1",
                NumberOfPieces = 1,
                Description = "cut key",
                CountryOfManufacturer = "US",
                CIMarksandNumbers = "87123",
                // ExportLicenseNumber = "26456",
                HarmonizedCode = shipment.DestinationAddress.IsCanadaAddress() ? "8301.70.900000" : "8301.70.000000",
                Quantity = 1,
                QuantityUnits = "EA",
                UnitPrice = 8.75m,
                CustomsValue = 8.75m,
                Amount = 8.75m,
                PartNumber = "167",
                //ExportLicenseExpirationDate = DateTime.Now.AddDays(1),
            });

        shipmentDetails.Commodities.Add(
            new Commodity()
            {
                Weight = 0.5m,
                Name = "key # 2",
                NumberOfPieces = 1,
                Description = "cut key",
                CountryOfManufacturer = "US",
                CIMarksandNumbers = "87123",
                // ExportLicenseNumber = "26456",
                HarmonizedCode = shipment.DestinationAddress.IsCanadaAddress() ? "8301.70.900000" : "8301.70.000000",
                Quantity = 1,
                QuantityUnits = "EA",
                UnitPrice = 8.75m,
                CustomsValue = 8.75m,
                Amount = 8.75m,
                PartNumber = "168",
                //ExportLicenseExpirationDate = DateTime.Now.AddDays(1),
            });

        var label = await _validator.CreateShipmentAsync(shipment, shipmentDetails, CancellationToken.None);

        Assert.Empty(label.InternalErrors);
        Assert.True(label?.Labels.Any(x => x?.Bytes?.Count > 0));
        var count = 0;
        foreach (var doc in label.Labels)
        {
            _output.WriteLine($"net charge : {doc.TotalCharges.NetCharge}, surcharge : {doc.TotalCharges.Surcharges} , basecharge : {doc.TotalCharges.BaseCharge}");
            // Path to save the PNG file
            var filePath = $"{nameof(DHLExpressShipmentProvider)}-{count}-label.{doc.ImageType}";

            // Write the byte array to a file
            File.WriteAllBytes(filePath, doc.Bytes.First());
            count++;
        }

        foreach (var doc in label.ShippingDocuments)
        {
            var filePath = $"{nameof(DHLExpressShipmentProvider)}-{doc.DocumentName}-{count}-label.{doc.ImageType}";

            // Write the byte array to a file
            File.WriteAllBytes(filePath, doc.Bytes.First());
            count++;
        }
    }
}
