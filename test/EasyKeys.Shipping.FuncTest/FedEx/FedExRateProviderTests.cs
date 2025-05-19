using Bet.Extensions.Testing.Logging;

using EasyKeys.Shipping.Abstractions;
using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.FedEx.Abstractions.Models;
using EasyKeys.Shipping.FedEx.Rates;

using EasyKeysShipping.FuncTest.TestHelpers;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EasyKeysShipping.FuncTest.FedEx;

public class FedExRateProviderTests
{
    private readonly ITestOutputHelper _output;

    private readonly Address _origin;

    private readonly IServiceProvider _sp;

    public FedExRateProviderTests(ITestOutputHelper output)
    {
        _output = output;
        _origin = new Address("11407 Granite St", "Charlotte", "NC", "28273", "US");
        _sp = ServiceProviderInstance.GetFedExServices(output);
    }

    public static IEnumerable<object[]> Data =>
        new List<object[]>
        {
                        new object[] { 0.5M, 2, FedExPackageType.FedExEnvelope.Name },
                        new object[] { 1M, 2, FedExPackageType.FedExEnvelope.Name },
                        new object[] { 1.05M, 2, FedExPackageType.FedExPak.Name },
                        new object[] { 2.1M, 2, FedExPackageType.FedExPak.Name },
                        new object[] { 3.3M, 2, FedExPackageType.FedExPak.Name },
                        new object[] { 3.75M, 2, FedExPackageType.FedExPak.Name },
                        new object[] { 33.3M, 2, FedExPackageType.FedExPak.Name },
        };

    [Theory]
    [MemberData(nameof(Data))]
    public async Task Return_FedEx_Rates_For_Envelope_Or_Pak_Successfully(decimal weight, int count, string packageType)
    {
        var rateServices = _sp.GetServices<IFedExRateProvider>();
        foreach (var rateService in rateServices)
        {
            var destination = new Address("152 Ski Cove Ln", "Hartsville", "SC", "29550", "US");
            var package = FedExRateConfigurator.GetFedExEnvelop(weight);

            var config = new FedExRateConfigurator(_origin, destination, package, true, DateTime.Now);

            var s = config.Shipments.Any(x => x.shipment.Options.PackagingType == packageType);
            Assert.True(s);

            Assert.Equal(count, config.Shipments.Count);

            foreach (var (shipment, serviceType) in config.Shipments)
            {
                var rates = await rateService.GetRatesAsync(shipment, serviceType);

                foreach (var rate in rates.Rates)
                {
                   // Assert.True(rates.Rates.Count > 8);
                    _output.WriteLine("{0}: {1}-{2}-{3}-{4}-{5}-{6}",rateService.GetType().FullName, rate.Name, rate.PackageType, rate.GuaranteedDelivery, rate.TotalCharges, rate.TotalCharges2, rate.SaturdayDelivery);
                }

                Assert.False(rates.InternalErrors.Any());
            }
        }
    }

    [Fact]
    public async Task Return_FedEx_Rates_For_All_Services_You_Packing_Successfully()
    {
        var rateServices = _sp.GetServices<IFedExRateProvider>();
        foreach (var rateService in rateServices)
        {
            var destination = new Address("750 County Road 456", "Jonesboro", "AR", "72404", "US");

            var packages = new List<Package>
            {
                // fedex envelope
                new Package(2m, 4m, 8m, 9m / 16, 20)
            };

            var shipOptions = new ShipmentOptions(FedExPackageType.YourPackaging.Name, DateTime.Now);

            var shipment = new Shipment(_origin, destination, packages, shipOptions);

            var rates = await rateService.GetRatesAsync(shipment);

            foreach (var rate in rates.Rates)
            {
               _output.WriteLine("{0}-{1}-{2}-{3}-{4}-{5}", rate.Name, rate.PackageType, rate.GuaranteedDelivery, rate.TotalCharges, rate.TotalCharges2, rate.SaturdayDelivery);
            }

            Assert.False(rates.InternalErrors.Any());

            //Assert.InRange(rates.Rates.Count, 1, 8);
        }
    }

    [Fact]
    public async Task Return_FedEx_Rates_For_FedEx_Ground_Package_Business_Address_Successfully()
    {
        var rateServices = _sp.GetServices<IFedExRateProvider>();
        foreach (var rateService in rateServices)
        {
            var destination = new Address("750 County Road 456", "Jonesboro", "AR", "72404", "US", isResidential: false);

            var packages = new List<Package>
            {
                // fedex envelope
                FedExRateConfigurator.GetFedExEnvelop(0.03125M)
            };

            var shipOptions = new ShipmentOptions(FedExPackageType.YourPackaging.Name, DateTime.Now.AddBusinessDays(1));

            var shipment = new Shipment(_origin, destination, packages, shipOptions);
            var rates = await rateService.GetRatesAsync(shipment, FedExServiceType.FedExGround);

            foreach (var rate in rates.Rates)
            {
               _output.WriteLine("{0}-{1}-{2}-{3}-{4}-{5}", rate.Name, rate.PackageType, rate.GuaranteedDelivery, rate.TotalCharges, rate.TotalCharges2, rate.SaturdayDelivery);
            }

            Assert.False(rates.InternalErrors.Any());

            // Assert.Single(rates.Rates);
        }
    }

    [Fact]
    public async Task Return_FedEx_Rates_For_FedEx_Pack_Business_Address_Successfully()
    {
        var rateServices = _sp.GetServices<IFedExRateProvider>();
        foreach (var rateService in rateServices)
        {
            var destination = new Address("750 County Road 456", "Jonesboro", "AR", "72404", "US", isResidential: false);

            var packages = new List<Package>
        {
            // fedex envelope
            FedExRateConfigurator.GetFedExEnvelop(1.03125M)
        };

            var shipOptions = new ShipmentOptions(FedExPackageType.FedExPak.Name, DateTime.Now.AddBusinessDays(1));

            var shipment = new Shipment(_origin, destination, packages, shipOptions);
            var rates = await rateService.GetRatesAsync(shipment);

            foreach (var rate in rates.Rates)
            {
               _output.WriteLine("{0}-{1}-{2}-{3}-{4}-{5}", rate.Name, rate.PackageType, rate.GuaranteedDelivery, rate.TotalCharges, rate.TotalCharges2, rate.SaturdayDelivery);
            }

            Assert.False(rates.InternalErrors.Any());

            Assert.True(rates.Rates.Count > 1);
        }
    }

    [Fact]
    public async Task Return_FedEx_Rates_For_Package_Residential_Address_Successfully()
    {
        var rateServices = _sp.GetServices<IFedExRateProvider>();
        foreach (var rateService in rateServices)
        {
            var destination = new Address("3000 Perimeter Park Dr", "Morrisville", "NC", "27560", "US", isResidential: false);

            var packages = new List<Package>
            {
                // fedex envelope
                FedExRateConfigurator.GetFedExEnvelop(0.03125M)
            };

            var shipOptions = new ShipmentOptions(FedExPackageType.YourPackaging.Name, DateTime.Now);

            var shipment = new Shipment(_origin, destination, packages, shipOptions);

            var rates = await rateService.GetRatesAsync(shipment);

            foreach (var rate in rates.Rates)
            {
                _output.WriteLine("{0}-{1}-{2}-{3}-{4}-{5}", rate.Name,rate.PackageType, rate.GuaranteedDelivery, rate.TotalCharges, rate.TotalCharges2, rate.SaturdayDelivery);
            }

            var shipOptions2 = new ShipmentOptions(FedExPackageType.FedExEnvelope.Name, DateTime.Now);

            var shipment2 = new Shipment(_origin, destination, packages, shipOptions2);

            var rates2 = await rateService.GetRatesAsync(shipment2);

            foreach (var rate in rates2.Rates)
            {
                _output.WriteLine("{0}-{1}-{2}-{3}-{4}-{5}", rate.Name, rate.PackageType, rate.GuaranteedDelivery, rate.TotalCharges, rate.TotalCharges2, rate.SaturdayDelivery);
            }

            Assert.False(rates.InternalErrors.Any());

            // Assert.Single(rates.Rates);
        }
    }

    [Fact]
    public async Task Return_FedEx_Rates_For_Intl_All_Services_You_Packing_Successfully()
    {
        var rateServices = _sp.GetServices<IFedExRateProvider>();

        foreach (var rateService in rateServices)
        {
            var destination = new Address("47 PEDMORE VALLEY", "NOTTINGHAM", string.Empty, "NG5 5NZ", "GB", isResidential: true);

            var packages = new List<Package>
            {
                // fedex envelope
                new Package(2m, 4m, 8m, 9m / 16, 20)
            };

            var shipOptions = new ShipmentOptions(FedExPackageType.YourPackaging.Name, DateTime.Now);

            var shipment = new Shipment(_origin, destination, packages, shipOptions);

            var rates = await rateService.GetRatesAsync(shipment);

            foreach (var rate in rates.Rates)
            {
                _output.WriteLine("{0}-{1}-{2}-{3}-{4}-{5}", rate.Name, rate.PackageType, rate.GuaranteedDelivery, rate.TotalCharges, rate.TotalCharges2, rate.SaturdayDelivery);
            }

            Assert.False(rates.InternalErrors.Any());

            // Assert.InRange(rates.Rates.Count, 1, 8);
        }
    }

    [Fact]
    public async Task Return_FedEx_Rate_For_Intl_Envelope_Successfully()
    {
        var rateServices = _sp.GetServices<IFedExRateProvider>();

        foreach (var rateService in rateServices)
        {
            var destination = TestShipments.CreateInternationalShipment().DestinationAddress;

            var packages = new List<Package>
            {
                // fedex envelope
               FedExRateConfigurator.GetFedExEnvelop(0.05M),
            };

            var shipOptions = new ShipmentOptions(FedExPackageType.FedExEnvelope.Name, DateTime.Now);

            var shipment = new Shipment(_origin, destination, packages, shipOptions);

            var rates = await rateService.GetRatesAsync(shipment);

            foreach (var rate in rates.Rates)
            {
                _output.WriteLine("{0}-{1}-{2}-{3}-{4}-{5}", rate.Name, rate.PackageType, rate.GuaranteedDelivery, rate.TotalCharges, rate.TotalCharges2, rate.SaturdayDelivery);
            }

            Assert.False(rates.InternalErrors.Any());
            // FEDEX INTERNATIONAL FIRST - 5 / 6 / 2021 10:00:00 AM - 125.26 - 125.26 - False
            // FEDEX INTERNATIONAL PRIORITY EXPRESS
            // FEDEX INTERNATIONAL PRIORITY - 5 / 6 / 2021 12:00:00 PM - 18.16 - 72.46 - False
            // FEDEX INTERNATIONAL ECONOMY - 5 / 11 / 2021 6:00:00 PM - 23.56 - 109.23 - False
            // FEDEX_INTERNATIONAL_CONNECT_PLUS-11/23/2022 10:00:00 PM-116.98-116.98-False
            // Assert.Equal(5, rates.Rates.Count);
        }
    }

    [Fact]
    public async Task Return_FedEx_Canada()
    {
        var rateServices = _sp.GetServices<IFedExRateProvider>();

        foreach (var rateService in rateServices)
        {
            var destination = new Address("550 WELLINGTON RD Dock G", "Honeywell Department", "LONDON", "ON", "N6C 0A7", "CA", isResidential: false);
            var package = FedExRateConfigurator.GetFedExEnvelop(0.03M, 7.90m, true);
            var config = new FedExRateConfigurator(_origin, destination, package, true, DateTime.Now);
            foreach (var (shipment, serviceType) in config.Shipments)
            {
                var result = await rateService.GetRatesAsync(shipment, serviceType);

                //Assert.True(result.Rates.Count > 0);

                foreach (var rate in result.Rates)
                {
                   _output.WriteLine("{0}-{1}-{2}-{3}-{4}-{5}", rate.Name, rate.PackageType, rate.GuaranteedDelivery, rate.TotalCharges, rate.TotalCharges2, rate.SaturdayDelivery);
                }

                Assert.False(result.InternalErrors.Any());
            }
        }
    }

    private ServiceProvider GetServices()
    {
        var services = new ServiceCollection();
        var dic = new Dictionary<string, string>
            {
                { "AzureVault:BaseUrl", "https://easykeys.vault.azure.net/" },
            };

        var configBuilder = new ConfigurationBuilder().AddInMemoryCollection(dic);

        configBuilder.AddAzureKeyVault(hostingEnviromentName: "Development", usePrefix: true);

        services.AddLogging(builder => builder.AddXunit(_output));
        services.AddSingleton<IConfiguration>(configBuilder.Build());
        services.AddWebServicesFedExRateProvider();

        return services.BuildServiceProvider();
    }
}
