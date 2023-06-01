using EasyKeys.Shipping.Stamps.Rates;
using EasyKeys.Shipping.Stamps.Rates.Models;

using EasyKeysShipping.FuncTest.TestHelpers;

using Microsoft.Extensions.DependencyInjection;

namespace EasyKeysShipping.FuncTest.Stamps;

public class StampsRateProviderTests
{
    private readonly IStampsRateProvider _rateProvider;
    private readonly ITestOutputHelper _output;

    public StampsRateProviderTests(ITestOutputHelper output)
    {
        _output = output;
        _rateProvider = ServiceProviderInstance.GetStampsServices(output)
            .GetRequiredService<IStampsRateProvider>();
    }

    [Fact]
    public async Task Return_Shipment_With_Rates_Successfully()
    {
        var result = await _rateProvider.GetRatesAsync(TestShipments.CreateDomesticShipment(), new RateOptions(), CancellationToken.None);

        Assert.NotNull(result);

        Assert.NotNull(result.Rates);

        Assert.Empty(result.InternalErrors);

        foreach (var rate in result.Rates)
        {
            _output.WriteLine($"{rate.ServiceName} - {rate.TotalCharges} / {rate.TotalCharges2}");
        }
    }
}
