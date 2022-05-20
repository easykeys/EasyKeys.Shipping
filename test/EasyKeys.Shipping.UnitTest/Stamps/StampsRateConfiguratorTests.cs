using System.Collections;

using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Rates;

using EasyKeysShipping.UnitTest.TestHelpers;

namespace EasyKeysShipping.UnitTest.Stamps;

public class StampsRateConfiguratorTests
{
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
                    }, .1m, StampsServiceType.FirstClass, PackageType.LargeEnvelopeOrFlat, "USPS First Class Mail"
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
                    }, .1m, StampsServiceType.FirstClassInternational, PackageType.Package, "USPS First Class Mail International"
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
                    }, 1m, StampsServiceType.Priority, PackageType.SmallFlatRateBox, "USPS Priority Mail"
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
                    }, 1m, StampsServiceType.FirstClassInternational, PackageType.Package, "USPS First Class Mail International"
            };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
