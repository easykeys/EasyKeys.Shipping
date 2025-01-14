using EasyKeys.Shipping.Amazon.Rates;
using EasyKeys.Shipping.Stamps.Shipment;

using EasyKeysShipping.FuncTest.TestHelpers;

using Microsoft.Extensions.DependencyInjection;

namespace EasyKeys.Shipping.FuncTest.Amazon;

public class AmazonShippingRateProviderTests
{
    private readonly IAmazonShippingRateProvider _rateProvider;
    private readonly ITestOutputHelper _output;

    public AmazonShippingRateProviderTests(ITestOutputHelper output)
    {
        _output = output;
        _rateProvider = ServiceProviderInstance.GetAmazonServices(output)
            .GetRequiredService<IAmazonShippingRateProvider>();
    }

    [Fact]
    public async Task Return_Shipment_With_Rates_Successfully()
    {
        var shipment = TestShipments.CreateDomesticShipment();
        var (sender, recipient) = TestShipments.CreateContactInfo();
        var result = await _rateProvider.GetRatesAsync(shipment, new () { SenderContact = sender, RecipientContact = recipient }, CancellationToken.None);

        Assert.NotNull(result);

        Assert.NotNull(result.Rates);

        Assert.Empty(result.InternalErrors);

        foreach (var rate in result.Rates)
        {
            _output.WriteLine($"{rate.ServiceName} - {rate.TotalCharges} / {rate.TotalCharges2}");
        }
    }
}
