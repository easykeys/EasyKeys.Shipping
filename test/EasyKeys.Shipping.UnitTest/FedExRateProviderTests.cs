using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Bet.Extensions.Testing.Logging;

using EasyKeys.Shipping.Abstractions;
using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.FedEx.Rates;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Xunit;
using Xunit.Abstractions;

namespace EasyKeysShipping.UnitTest
{
    public class FedExRateProviderTests
    {
        private readonly ITestOutputHelper _output;

        private readonly Address _origin;

        private readonly ServiceProvider _sp;

        public FedExRateProviderTests(ITestOutputHelper output)
        {
            _output = output;
            _origin = new Address("11407 Granite St", "Charlotte", "NC", "28273", "US");
            _sp = GetServices();
        }

        public static IEnumerable<object[]> Data =>
            new List<object[]>
            {
                        new object[] { 0.5M, 2, nameof(FedExPackageType.FEDEX_ENVELOPE) },
                        new object[] { 1M, 2, nameof(FedExPackageType.FEDEX_ENVELOPE) },
                        new object[] { 1.05M, 2, nameof(FedExPackageType.FEDEX_PAK) },
                        new object[] { 2.1M, 2, nameof(FedExPackageType.FEDEX_PAK) },
                        new object[] { 3.3M, 2, nameof(FedExPackageType.FEDEX_PAK) },
                        new object[] { 3.75M, 2, nameof(FedExPackageType.FEDEX_PAK) },
                        new object[] { 33.3M, 2, nameof(FedExPackageType.FEDEX_PAK) },
            };

        [Theory]
        [MemberData(nameof(Data))]
        public async Task Return_FedEx_Rates_For_Envelope_Or_Pak_Successfully(decimal weight, int count, string packageType)
        {
            var rateService = _sp.GetRequiredService<IFedExRateProvider>();
            var destination = new Address("152 Ski Cove Ln", "Hartsville", "SC", "29550", "US");
            var package = FedExRateConfigurator.GetFedExEnvelop(weight);

            var config = new FedExRateConfigurator(_origin, destination, package, DateTime.Now);

            var s = config.Shipments.Any(x => x.shipment.Options.PackagingType == packageType);
            Assert.True(s);

            Assert.Equal(count, config.Shipments.Count);

            foreach (var (shipment, serviceType) in config.Shipments)
            {
                var rates = await rateService.GetRatesAsync(shipment, serviceType);

                foreach (var rate in rates.Rates)
                {
                    Assert.InRange(rates.Rates.Count, 1, 8);
                    _output.WriteLine("{0} - {1} - ${2} - ${3} - {4}", rate.Name, rate.GuaranteedDelivery, rate.TotalCharges, rate.TotalCharges2, rate.SaturdayDelivery);
                }
            }
        }

        [Fact]
        public async Task Return_FedEx_Rates_For_All_Services_You_Packing_Successfully()
        {
            var rateService = _sp.GetRequiredService<IFedExRateProvider>();
            var destination = new Address("750 County Road 456", "Jonesboro", "AR", "72404", "US");

            var packages = new List<Package>
            {
                // fedex envelope
                new Package(2m, 4m, 8m, 9m / 16,20)
            };

            var shipOptions = new ShipmentOptions
            {
                PackagingType = nameof(FedExPackageType.YOUR_PACKAGING),
                SaturdayDelivery = true,
                ShippingDate = DateTime.Now,
                PreferredCurrencyCode = ShipmentOptions.DefaultCurrencyCode
            };

            var shipment = new Shipment(_origin, destination, packages, shipOptions);

            var rates = await rateService.GetRatesAsync(shipment);

            foreach (var rate in rates.Rates)
            {
                _output.WriteLine("{0}-{1}-{2}-{3}-{4}", rate.Name, rate.GuaranteedDelivery, rate.TotalCharges, rate.TotalCharges2, rate.SaturdayDelivery);
            }

            Assert.InRange(rates.Rates.Count, 1, 8);
        }

        [Fact]
        public async Task Return_FedEx_Rates_For_FedEx_Ground_Package_Business_Address_Successfully()
        {
            var rateService = _sp.GetRequiredService<IFedExRateProvider>();
            var destination = new Address("750 County Road 456", "Jonesboro", "AR", "72404", "US", isResidential: false);

            var packages = new List<Package>
            {
                // fedex envelope
                FedExRateConfigurator.GetFedExEnvelop(0.03125M)
            };

            var shipOptions = new ShipmentOptions
            {
                PackagingType = nameof(FedExPackageType.YOUR_PACKAGING),
                SaturdayDelivery = true,
                ShippingDate = DateTime.Now.AddBusinessDays(1),
                PreferredCurrencyCode = ShipmentOptions.DefaultCurrencyCode,
            };

            var shipment = new Shipment(_origin, destination, packages, shipOptions);
            var rates = await rateService.GetRatesAsync(shipment, ServiceType.FEDEX_GROUND);

            foreach (var rate in rates.Rates)
            {
                _output.WriteLine("{0}-{1}-{2}-{3}-{4}", rate.Name, rate.GuaranteedDelivery, rate.TotalCharges, rate.TotalCharges2, rate.SaturdayDelivery);
            }

            Assert.Single(rates.Rates);
        }

        [Fact]
        public async Task Return_FedEx_Rates_For_FedEx_Pack_Business_Address_Successfully()
        {
            var rateService = _sp.GetRequiredService<IFedExRateProvider>();
            var destination = new Address("750 County Road 456", "Jonesboro", "AR", "72404", "US", isResidential: false);

            var packages = new List<Package>
            {
                // fedex envelope
                FedExRateConfigurator.GetFedExEnvelop(1.03125M)
            };

            var shipOptions = new ShipmentOptions
            {
                PackagingType = nameof(FedExPackageType.FEDEX_PAK),
                SaturdayDelivery = true,
                ShippingDate = DateTime.Now.AddBusinessDays(1),
                PreferredCurrencyCode = ShipmentOptions.DefaultCurrencyCode,
            };

            var shipment = new Shipment(_origin, destination, packages, shipOptions);
            var rates = await rateService.GetRatesAsync(shipment);

            foreach (var rate in rates.Rates)
            {
                _output.WriteLine("{0}-{1}-{2}-{3}-{4}", rate.Name, rate.GuaranteedDelivery, rate.TotalCharges, rate.TotalCharges2, rate.SaturdayDelivery);
            }

            Assert.True(rates.Rates.Count > 1);
        }

        [Fact]
        public async Task Return_FedEx_Rates_For_Package_Residential_Address_Successfully()
        {
            var rateService = _sp.GetRequiredService<IFedExRateProvider>();
            var destination = new Address("750 County Road 456", "Jonesboro", "AR", "72404", "US", isResidential: true);

            var packages = new List<Package>
            {
                // fedex envelope
                FedExRateConfigurator.GetFedExEnvelop(0.03125M)
            };

            var shipOptions = new ShipmentOptions
            {
                PackagingType = nameof(FedExPackageType.YOUR_PACKAGING),
                SaturdayDelivery = true,
                ShippingDate = DateTime.Now.AddBusinessDays(1),
                PreferredCurrencyCode = ShipmentOptions.DefaultCurrencyCode,
            };

            var shipment = new Shipment(_origin, destination, packages, shipOptions);

            var rates = await rateService.GetRatesAsync(shipment, ServiceType.GROUND_HOME_DELIVERY);

            foreach (var rate in rates.Rates)
            {
                _output.WriteLine("{0}-{1}-{2}-{3}-{4}", rate.Name, rate.GuaranteedDelivery, rate.TotalCharges, rate.TotalCharges2, rate.SaturdayDelivery);
            }

            Assert.Single(rates.Rates);
        }

        [Fact]
        public async Task Return_FedEx_Rates_For_Intl_All_Services_You_Packing_Successfully()
        {
            var rateService = _sp.GetRequiredService<IFedExRateProvider>();

            var destination = new Address("47 PEDMORE VALLEY", "NOTTINGHAM", string.Empty, "NG5 5NZ", "GB", isResidential: true);

            var packages = new List<Package>
            {
                // fedex envelope
                new Package(2m, 4m, 8m, 9m / 16,20)
            };

            var shipOptions = new ShipmentOptions
            {
                PackagingType = nameof(FedExPackageType.YOUR_PACKAGING),
                SaturdayDelivery = true,
                ShippingDate = DateTime.Now,
                PreferredCurrencyCode = ShipmentOptions.DefaultCurrencyCode
            };

            var shipment = new Shipment(_origin, destination, packages, shipOptions);

            var rates = await rateService.GetRatesAsync(shipment);

            foreach (var rate in rates.Rates)
            {
                _output.WriteLine("{0}-{1}-{2}-{3}-{4}", rate.Name, rate.GuaranteedDelivery, rate.TotalCharges, rate.TotalCharges2, rate.SaturdayDelivery);
            }

            Assert.InRange(rates.Rates.Count, 1, 8);
        }

        [Fact]
        public async Task Return_FedEx_Rate_For_Intl_Envelope_Successfully()
        {
            var rateService = _sp.GetRequiredService<IFedExRateProvider>();

            var destination = new Address("47 PEDMORE VALLEY", "NOTTINGHAM", string.Empty, "NG5 5NZ", "GB", isResidential: true);

            var packages = new List<Package>
            {
                // fedex envelope
               FedExRateConfigurator.GetFedExEnvelop(0.05M),
            };

            var shipOptions = new ShipmentOptions
            {
                PackagingType = nameof(FedExPackageType.FEDEX_ENVELOPE),
                SaturdayDelivery = true,
                ShippingDate = DateTime.Now,
                PreferredCurrencyCode = ShipmentOptions.DefaultCurrencyCode
            };

            var shipment = new Shipment(_origin, destination, packages, shipOptions);

            var rates = await rateService.GetRatesAsync(shipment);

            foreach (var rate in rates.Rates)
            {
                _output.WriteLine("{0}-{1}-{2}-{3}-{4}", rate.Name, rate.GuaranteedDelivery, rate.TotalCharges, rate.TotalCharges2, rate.SaturdayDelivery);
            }

            // FEDEX INTERNATIONAL FIRST - 5 / 6 / 2021 10:00:00 AM - 125.26 - 125.26 - False
            // FEDEX INTERNATIONAL PRIORITY - 5 / 6 / 2021 12:00:00 PM - 18.16 - 72.46 - False
            // FEDEX INTERNATIONAL ECONOMY - 5 / 11 / 2021 6:00:00 PM - 23.56 - 109.23 - False
            Assert.Equal(3, rates.Rates.Count);
        }

        [Fact]
        public async Task Return_FedEx_Canada()
        {
            var rateService = _sp.GetRequiredService<IFedExRateProvider>();
            var destination = new Address("550 WELLINGTON RD Dock G", "Honeywell Department", "", "LONDON", "ON", "N6C 0A7", "CA", isResidential: false);
            var package = FedExRateConfigurator.GetFedExEnvelop(0.03M, 7.90m, true);
            var config = new FedExRateConfigurator(_origin, destination, package, DateTime.Now);
            foreach (var (shipment, serviceType) in config.Shipments)
            {
                var result = await rateService.GetRatesAsync(shipment, serviceType);
                Assert.True(result.Rates.Count > 0);
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
            services.AddFedExRateProvider();

            return services.BuildServiceProvider();
        }
    }
}
