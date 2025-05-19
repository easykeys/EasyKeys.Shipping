using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.DHL.Rates;

using EasyKeysShipping.FuncTest.TestHelpers;

using Microsoft.Extensions.DependencyInjection;

namespace EasyKeysShipping.FuncTest.DHL;

public class DHLRateProviderTests
{
    private readonly ITestOutputHelper _output;
    private readonly IEnumerable<IDHLExpressRateProvider> _validators;

    public DHLRateProviderTests(ITestOutputHelper output)
    {
        _output = output;
        _validators = ServiceProviderInstance.GetDHLServices(output).GetServices<IDHLExpressRateProvider>();
    }

    [Fact]
    public async Task Get_International_Rates_Successfully()
    {
        var requests = new List<Shipment>
        {
            TestShipments.CreateInternationalShipment(),
        };

        foreach (var request in requests)
        {
            foreach (var validator in _validators)
            {
                var rates = await validator.GetRatesAsync(request);

                foreach (var rate in rates.Rates)
                {
                    _output.WriteLine("{0}-{1}-{2}-{3}-{4}-{5}", rate.ServiceName, rate.PackageType, rate.GuaranteedDelivery, rate.TotalCharges, rate.TotalCharges2, rate.SaturdayDelivery);
                }

                request.Rates.Clear();

                rates = await validator.GetRatesManyAsync(request);
                foreach (var rate in rates.Rates)
                {
                    _output.WriteLine("{0}-{1}-{2}-{3}-{4}-{5}", rate.ServiceName, rate.PackageType, rate.GuaranteedDelivery, rate.TotalCharges, rate.TotalCharges2, rate.SaturdayDelivery);
                }
            }
        }
    }

    [Fact]
    public async Task Get_Domestic_Rates_Successfully()
    {
        var requests = new List<Shipment>
        {
            TestShipments.CreateDomesticShipment(),
        };

        foreach (var request in requests)
        {
            foreach (var validator in _validators)
            {
                var rates = await validator.GetRatesAsync(request);

                foreach (var rate in rates.Rates)
                {
                    _output.WriteLine("{0}-{1}-{2}-{3}-{4}-{5}", rate.Name, rate.PackageType, rate.GuaranteedDelivery, rate.TotalCharges, rate.TotalCharges2, rate.SaturdayDelivery);
                }
            }
        }
    }
}
