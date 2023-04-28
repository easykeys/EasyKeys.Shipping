using EasyKeys.Shipping.Stamps.Rates;
using EasyKeys.Shipping.Stamps.Rates.Models;

using EasyKeysShipping.FuncTest.TestHelpers;

using Microsoft.Extensions.DependencyInjection;

namespace EasyKeysShipping.FuncTest.Stamps;

public class StampsRateProviderTests
{
    private readonly IStampsRateProvider _rateProvider;

    public StampsRateProviderTests(ITestOutputHelper output)
    {
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
    }
}
