using System.ComponentModel.DataAnnotations;
using System.Reflection.Emit;

using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.DHL.Shipment;

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

        var label = await _validator.CreateShipmentAsync(TestShipments.CreateInternationalShipment());

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
