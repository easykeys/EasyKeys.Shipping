using System.Collections;

using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Models.Enums.ServiceTypes;
using EasyKeys.Shipping.Stamps.Rates;
using EasyKeys.Shipping.Stamps.Rates.Extensions;

using EasyKeysShipping.UnitTest.TestHelpers;

namespace EasyKeysShipping.UnitTest.Stamps
{
    public class StampsRateConfiguratorTests
    {
        [Theory]
        [ClassData(typeof(StampConfigTypeData))]
        public void Return_Correct_RateRequestDetails_Successfully(
            Address address,
            decimal weight,
            ServiceTypes type,
            PackageType packageType,
            string serviceDescription)
        {
            var shipment = TestShipments.CreateDomesticShipment();
            var config = new StampsRateConfigurator(
                shipment.OriginAddress,
                address,
                new Package(1m, 1m, 1m, weight, 20m),
                shipment.SenderInfo,
                shipment.RecipientInfo);

            Assert.Contains(config.Shipments, (x) => x.rateOptions.ServiceType == type);
            Assert.Contains(config.Shipments, (x) => x.rateOptions.PackageType == packageType);
            Assert.Contains(config.Shipments, (x) => x.rateOptions.ServiceDescription == serviceDescription);
        }

        [Fact]
        public void Return_Correct_FlatRatePackage_Successfully()
        {
            Assert.True(new Package(12.5m, 9.5m, 4m, .81m, 20m).IsPaddedFlatRateEnvelope());

            Assert.True(new Package(8.625m, 5.375m, 1.62m, 10m, 20m).IsSmallFlatRateBox());

            Assert.True(new Package(11m, 8.5m, 5.5m, 10m, 20m).IsFlatRateBox());

            Assert.True(new Package(12m, 12m, 6m, 10m, 20m).IsLargeFlatRateBox());
        }

        [Fact]
        public void Throw_Exception_When_Weight_Is_Greater_Than_70lbs()
        {
            var shipment = TestShipments.CreateDomesticShipment();

            Assert.Throws<ArgumentException>(() => new StampsRateConfigurator(
                shipment.OriginAddress,
                shipment.DestinationAddress,
                new Package(1m, 1m, 1m, 80m, 20m),
                shipment.SenderInfo,
                shipment.RecipientInfo));
        }

        public class StampConfigTypeData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[]
                {
                    new Address()
                        {
                            StreetLine = "1550 Central Ave",
                            StreetLine2 = "Apt 35",
                            City = "Riverside",
                            StateOrProvince = "CA",
                            CountryCode = "US",
                            PostalCode = "92507"
                        }, .1m, ServiceTypes.USPS_First_Class_Mail, PackageType.Large_Envelope_Or_Flat, "USPS First Class Mail"
                };
                yield return new object[]
                {
                    new Address()
                        {
                            City = "San Diana",
                            StreetLine = "Strada Gilda 2 Piano 9",
                            StateOrProvince = "Brescia",
                            CountryCode = "IT",
                            PostalCode = "64921"
                        }, .1m, ServiceTypes.USPS_First_Class_Mail_International, PackageType.Package, "USPS First Class Mail International"
                };
                yield return new object[]
                {
                    new Address()
                        {
                            StreetLine = "1550 Central Ave",
                            StreetLine2 = "Apt 35",
                            City = "Riverside",
                            StateOrProvince = "CA",
                            CountryCode = "US",
                            PostalCode = "92507"
                        }, 1m, ServiceTypes.USPS_Priority_Mail, PackageType.Small_Flat_Rate_Box, "USPS Priority Mail"
                };
                yield return new object[]
                {
                    new Address()
                        {
                            City = "San Diana",
                            StreetLine = "Strada Gilda 2 Piano 9",
                            StateOrProvince = "Brescia",
                            CountryCode = "IT",
                            PostalCode = "64921"
                        }, 1m, ServiceTypes.USPS_First_Class_Mail_International, PackageType.Package, "USPS First Class Mail International"
                };
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
