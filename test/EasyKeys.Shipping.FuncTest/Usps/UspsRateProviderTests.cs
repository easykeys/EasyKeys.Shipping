using EasyKeys.Shipping.Abstractions;
using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Usps.Rates;

using EasyKeysShipping.FuncTest.TestHelpers;

using Microsoft.Extensions.DependencyInjection;

namespace EasyKeysShipping.FuncTest.Usps;

public class UspsRateProviderTests
{
    private readonly ITestOutputHelper _output;
    private readonly Address _origin;
    private readonly ServiceProvider _sp;
    private readonly IUspsRateProvider _provider;

    public UspsRateProviderTests(ITestOutputHelper output)
    {
        _output = output;
        _origin = new Address("11407 Granite St", "Charlotte", "NC", "28273", "US");

        _sp = ServiceProviderInstance.GetUspsServices(output);

        _provider = _sp.GetRequiredService<IUspsRateProvider>();
    }

    public static IEnumerable<object[]> EnvelopeData =>
       new List<object[]>
       {
            // max for all parameters
            new object[] { new Package(15m, 12m, 0.75m, 15m / 16, 0m), true, true },

            // min for all parameters
            new object[] { new Package(11.50m, 6.125m, 0.25m, 0.125m, 0m), true, true },

            new object[] { new Package(15m, 7m, 0.25m, 0.125m, 0m), true, true },

            // width is false, but weight is true
            new object[] { new Package(15m, 13m, 0.25m, 0.125m, 0m), false, true },
            new object[] { new Package(15m, 6m, 0.25m, 0.125m, 0m), false, true },

            // height (thickness) is false, but weight is true
            new object[] { new Package(11.5m, 7m, 0.85m, 0.125m, 0m), false, true },
            new object[] { new Package(11.5m, 7m, 0.15m, 0.125m, 0m), false, true },

            // length is false and weight is false
            new object[] { new Package(15.5m, 12m, 0.75m, 0.98m, 0m), false, false },
            new object[] { new Package(11m, 12m, 0.75m, 0.98m, 0m), false, false },
       };

    public static IEnumerable<object[]> PriorityData =>
       new List<object[]>
       {
                    new object[] { 1.2M },
                    new object[] { 2.1M },
                    new object[] { 3.3M },
                    new object[] { 3.75M },
                    new object[] { 33.3M },
       };

    public static IEnumerable<object[]> FistClassData =>
        new List<object[]>
        {
                        new object[] { 1.0M, 5.00m },
                        new object[] { 5.0M, 5.60m },
                        new object[] { 10.0M, 6.35m },
                        new object[] { 13.0M, 7.85m },
                        new object[] { 15.0M, 7.85m },
        };

    [Theory]
    [MemberData(nameof(EnvelopeData))]
    public void Should_Be_Envelop(Package package, bool d, bool w)
    {
        // 2 oz = 0.125
        Assert.Equal(d, package.IsEnvelope());
        Assert.Equal(w, package.IsEnvelopeWeight());
    }

    [Theory]
    [MemberData(nameof(PriorityData))]
    public async Task Get_Priority_Mail_Small_Flat_Rate_Box_Successfully(decimal weight)
    {
        // USPS Priority Mail 2-Day Small Flat Rate Box- $8.45 stamps.com is $7.90
        var destination = new Address("5204 E BROADWAY AVE", "SPOKANE Valley", "WA", "99212", "US", isResidential: true);
        var package = UspsRateConfigurator.GetSmallFlatBox(weight);
        var config = new UspsRateConfigurator(_origin, destination, package, DateTime.Now);
        var (shipment, rateOptions) = config.Shipments[0];

        var result = await _provider.GetRatesAsync(shipment, rateOptions);
        var rate = result.Rates.FirstOrDefault();

        Assert.NotNull(rate);

        // usps rate
        Assert.Equal(10.20m, rate?.TotalCharges);

        // stamps.com rate
        Assert.Equal(7.90m, config.StampsRate);
        _output.WriteLine($"{rate?.ServiceName} - ${rate?.TotalCharges} - {rate?.TotalCharges2} - {rate?.GuaranteedDelivery}");
    }

    [Theory]
    [MemberData(nameof(PriorityData))]
    public async Task Get_Priority_Mail_Medium_Flat_Rate_Box_Successfully(decimal weight)
    {
        // use case for this shipment can be:
        // https://easykeys.com/1749_LAB_Mini_DUR-X_Schlage_Rekeying_Kit_LMDSCH.aspx

        // 9405511298370116099860 Flat Rate Box 3 lbs. 12 oz. $13.75
        // USPS Priority Mail 2-Day Medium Flat Rate Box- $15.50
        var destination = new Address("5204 E BROADWAY AVE", "SPOKANE Valley", "WA", "99212", "US", isResidential: true);
        var package = UspsRateConfigurator.GetMediumFlatBox(weight);
        var config = new UspsRateConfigurator(_origin, destination, package, DateTime.Now);
        var (shipment, rateOptions) = config.Shipments[0];

        var result = await _provider.GetRatesAsync(shipment, rateOptions);
        var rate = result.Rates.FirstOrDefault();

        Assert.NotNull(rate);
        Assert.Equal(17.10m, rate?.TotalCharges);
        Assert.Equal(13.75m, config.StampsRate);

        _output.WriteLine($"{rate?.ServiceName} - ${rate?.TotalCharges} - {rate?.TotalCharges2} - {rate?.GuaranteedDelivery}");
    }

    [Theory]
    [MemberData(nameof(FistClassData))]
    public async Task Get_First_Class_Large_Envelope_13oz_Successfully(decimal weightInOz, decimal charge)
    {
        // https://www.stamps.com/usps/first-class-package-service/
        // USPS First-Class Package Service - Retail- $4.15 weight .0125M
        var destination = new Address("229 S GREEN ST", "HENDERSON", "KY", "42420-3540", "US", isResidential: true);
        var weight = weightInOz / 16;
        var package = UspsRateConfigurator.GetFirstClassPackage(weight);
        var config = new UspsRateConfigurator(_origin, destination, package, DateTime.Now);
        var (shipment, rateOptions) = config.Shipments[0];

        var result = await _provider.GetRatesAsync(shipment, rateOptions);
        var rate = result.Rates.FirstOrDefault();

        Assert.NotNull(rate);
        Assert.Equal(charge, rate?.TotalCharges);

        _output.WriteLine($"{rate?.ServiceName} - ${rate?.TotalCharges} - {rate?.GuaranteedDelivery}");
    }

    [Theory]
    [MemberData(nameof(FistClassData))]
    public async Task Get_First_Class_And_Priority_Mail_Successfully(decimal weightInOz, decimal charge)
    {
        // https://www.stamps.com/usps/first-class-package-service/
        // USPS First-Class Package Service - Retail- $4.15 weight .0125M
        var destination = new Address("229 S GREEN ST", "HENDERSON", "KY", "42420-3540", "US", isResidential: true);
        var weight = weightInOz / 16;
        var package = UspsRateConfigurator.GetFirstClassPackage(weight);
        var configs = new UspsRateConfigurator(_origin, destination, package, DateTime.Now);
        foreach (var (shipment, rateOptions) in configs.Shipments)
        {
            var result = await _provider.GetRatesAsync(shipment, rateOptions);
            var rate = result.Rates.FirstOrDefault();
            Assert.NotNull(rate);

            // Assert.Equal(charge, rate.TotalCharges);
            _output.WriteLine($"{rate?.ServiceName} - ${rate?.TotalCharges} - {rate?.GuaranteedDelivery}");
        }
    }

    [Fact]
    public async Task Get_First_Class_Package_2oz_Successfully()
    {
        // https://www.stamps.com/usps/first-class-package-service/
        // USPS First-Class Package Service - Retail- $4.15 weight .0125M
        var destination = new Address(" 6828 ARJAY DR", "INDIANAPOLIS", "IN", "46217 - 3001", "US", isResidential: true);

        // USPS First-Class Package Service - Retail-6.40- PACKAGE_SERVICE_RETAIL
        // USPS First-Class Mail Large Envelope - 3.40 - FLAT
        var weight = 2.00m / 16;
        var package = UspsRateConfigurator.GetFirstClassPackage(weight);
        var config = new UspsRateConfigurator(_origin, destination, package, DateTime.Now);
        var (shipment, rateOptions) = config.Shipments[0];

        var result = await _provider.GetRatesAsync(shipment, rateOptions);
        var rate = result.Rates.FirstOrDefault();

        Assert.NotNull(rate);
        Assert.Equal(5.0m, rate?.TotalCharges);
        _output.WriteLine($"{rate?.ServiceName} - ${rate?.TotalCharges} - {rate?.GuaranteedDelivery}");
    }

    [Theory]

    // USPS First-Class Package International Service- $42.65 for 4 lbs
    [InlineData("2.4", "45.15")]

    // USPS First-Class Package International Service- $58.35
    [InlineData("4", "61.80")]
    public async Task Get_First_Class_International_Large_Package_Successfully(string w, string c)
    {
        var weight = Convert.ToDecimal(w);
        var charge = Convert.ToDecimal(c);

        var destination = new Address("1851, BUR JUMAN BUSINESS TOWER,MAKHOOL ", "Dubai", string.Empty, string.Empty, "AE", isResidential: true);

        // CASE: First Class International (R) Large Envelope or Flat $36.56 weight 2 lbs 4 oz
        var packages = new List<Package>
            {
                UspsRateConfigurator.GetIntlEnvelope(weight)
            };

        var shipOptions = new ShipmentOptions(nameof(IntlPackageType.LARGEENVELOPE), DateTime.Now.AddBusinessDays(1));
        var shipment = new Shipment(_origin, destination, packages, shipOptions);
        var rateOptions = new UspsRateOptions { ServiceName = "First-Class Package International Service" };

        var result = await _provider.GetRatesAsync(shipment, rateOptions);
        var rate = result.Rates.FirstOrDefault();

        Assert.NotNull(rate);
        Assert.Equal(charge, rate?.TotalCharges);
        Assert.Null(rate?.GuaranteedDelivery);

        _output.WriteLine($"{rate?.ServiceName} - ${rate?.TotalCharges} - {rate?.GuaranteedDelivery}");
    }

    [Theory]

    // USPS First-Class Package International Service- $42.65 for 2.4 lbs
    [InlineData("2.4", "45.15")]

    // USPS First-Class Package International Service- $58.35
    [InlineData("4", "61.80")]

    // USPS First-Class Package International Service- $16.45 with 2 oz or 0.125 lbs
    [InlineData("0.125", "17.40")]
    public async Task Get_First_Class_International_Mail_Successfully(string w, string c)
    {
        var weight = Convert.ToDecimal(w);
        var charge = Convert.ToDecimal(c);

        var destination = new Address("1851, BUR JUMAN BUSINESS TOWER,MAKHOOL ", "Dubai", string.Empty, string.Empty, "AE", isResidential: true);

        // CASE: UM606525025US First Class International (R) Large Envelope or Flat $36.56 weight 2 lbs 4 oz
        var packages = new List<Package>
            {
                // weight from 1 oz to 4lbs for First-Class Package International Service
                UspsRateConfigurator.GetIntlBox(weight)
            };

        var shipOptions = new ShipmentOptions(nameof(IntlPackageType.ALL), DateTime.Now.AddBusinessDays(1));

        var shipment = new Shipment(_origin, destination, packages, shipOptions);
        var rateOptions = new UspsRateOptions { ServiceName = "First-Class Package International Service" };

        var result = await _provider.GetRatesAsync(shipment, rateOptions);
        var rate = result.Rates.FirstOrDefault();
        Assert.NotNull(rate);

        // expectable rate for this package due to the weight
        Assert.Equal(charge, rate?.TotalCharges);
        Assert.Null(rate?.GuaranteedDelivery);
        _output.WriteLine($"{rate?.ServiceName} - ${rate?.TotalCharges} - {rate?.GuaranteedDelivery}");
    }

    [Fact]
    public async Task Get_Priority_Mail_International_Package_5lbs_Successfully()
    {
        var destination = new Address("FLAT 22 TOLBUT COURT", "ROMFORD", string.Empty, string.Empty, "GB", isResidential: true);

        var packages = new List<Package>
            {
                // https://www.usps.com/international/priority-mail-international.htm
                // Maximum weight for Priority Mail International Flat Rate Envelopes and small Flat Rate Boxes is 4 lbs.
                // Fast international delivery time: 6–10 business days
                // new Package(11M, 5M, 1M, 5M, 0.0M),
                UspsRateConfigurator.GetIntlBox(5m)
            };

        var shipOptions = new ShipmentOptions(nameof(IntlPackageType.PACKAGE), DateTime.Now.AddBusinessDays(1));

        var shipment = new Shipment(_origin, destination, packages, shipOptions);
        var rateOptions = new UspsRateOptions { ServiceName = "Priority Mail International" };

        var result = await _provider.GetRatesAsync(shipment, rateOptions);
        var rate = result.Rates.FirstOrDefault();

        Assert.NotNull(rate);
        Assert.Equal(83.95m, rate?.TotalCharges);

        _output.WriteLine($"{rate?.ServiceName}- ${rate?.TotalCharges} - {rate?.GuaranteedDelivery}");
    }

    [Theory]

    // USPS First-Class Package International Service- $48.00 for 2.4 lbs
    [InlineData("2.4", "48.00")]

    // USPS First-Class Package International Service - $29.75
    [InlineData("1.60", "29.75")]

    // USPS First-Class Package International Service- $18.25 with 2 oz or 0.125 lbs
    [InlineData("0.125", "18.25")]
    public async Task Get_First_Class_International_Package_Successfully(string w, string c)
    {
        var weight = Convert.ToDecimal(w);
        var charge = Convert.ToDecimal(c);

        // dest: "1/2495 Big River Way, ULMARRA, AU"
        var destination = new Address("1/2495 Big River Way", "ULMARRA", string.Empty, string.Empty, "AU", isResidential: true);

        // CASE: $25.05
        // USPS Priority Mail International - $61.40
        // 1 lbs 6oz (25.60oz)
        var package = UspsRateConfigurator.GetIntlBox(weight);
        var config = new UspsRateConfigurator(_origin, destination, package, DateTime.Now);
        var (shipment, rateOptions) = config.Shipments[0];

        var result = await _provider.GetRatesAsync(shipment, rateOptions);
        var rate = result.Rates.FirstOrDefault();
        Assert.NotNull(rate);

        // Assert.Equal(29.75m, rate.TotalCharges);
        _output.WriteLine($"{rate?.ServiceName} - ${rate?.TotalCharges} - {rate?.GuaranteedDelivery}");
    }
}
