using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Rates;

namespace EasyKeysShipping.UnitTest.Stamps;

public class StampsRateConfiguratorTests
{
    private readonly ITestOutputHelper _output;
    private readonly Address _originAddress;

    public StampsRateConfiguratorTests(ITestOutputHelper output)
    {
        _output = output;
        _originAddress = new Address(
                 streetLine: "11407 Granite Street",
                 city: "Charlotte",
                 stateOrProvince: "NC",
                 postalCode: "28273",
                 countryCode: "US");
    }

    [Theory]
    [InlineData(8.0, 4.0, 0.75, 15.99, 7)]
    [InlineData(8.0, 4.0, 0.75, 5, 8)]
    [InlineData(8.0, 4.0, 0.75, 0.5, 9)]
    [InlineData(6.0, 2.0, 2, 4, 8)]
    [InlineData(11.0, 8.0, 2, 32, 5)]
    [InlineData(14.0, 10.0, 3, 144, 4)]
    [InlineData(20.0, 20.0, 20, 800, 0)]
    public void Check_Domestic(decimal l, decimal w, decimal h, decimal oz, int count)
    {
        var destinationAddress = new Address(
                        streetLine: "1550 central avenue",
                        city: "riverside",
                        stateOrProvince: "CA",
                        postalCode: "92507",
                        countryCode: "US");

        var weight = new PackageWeight(oz, isOunce: true);

        var packages = new List<Package>
        {
            new Package(
            new Dimensions()
            {
                Length = l,
                Width = w,
                Height = h,
            },
            weight.InPounds),
        };

        var configurator = new StampsRateConfigurator(_originAddress, destinationAddress, packages[0]);

        Assert.NotNull(configurator);

        foreach (var item in configurator.Shipments)
        {
            _output.WriteLine(item.Options.PackagingType);
        }

        Assert.Equal(count, configurator.Shipments.Count);
    }

    [Theory]
    [InlineData(8.0, 4.0, 0.75, 15.99, 7)]
    [InlineData(8.0, 4.0, 0.75, 5, 8)]
    [InlineData(8.0, 4.0, 0.75, 0.5, 9)]
    [InlineData(6.0, 2.0, 2, 4, 8)]
    [InlineData(11.0, 8.0, 2, 32, 5)]
    [InlineData(14.0, 10.0, 3, 144, 3)]
    [InlineData(20.0, 20.0, 20, 800, 0)]
    public void Check_International(decimal l, decimal w, decimal h, decimal oz, int count)
    {
        var destinationAddress = new Address(
                        streetLine: "512 Venture Place",
                        city: "Barrhead",
                        stateOrProvince: "AB",
                        postalCode: "T0G 0E0",
                        countryCode: "CA");

        var weight = new PackageWeight(oz, isOunce: true);

        var packages = new List<Package>
        {
            new Package(
            new Dimensions()
            {
                Length = l,
                Width = w,
                Height = h,
            },
            weight.InPounds),
        };

        var configurator = new StampsRateConfigurator(_originAddress, destinationAddress, packages[0]);

        Assert.NotNull(configurator);

        foreach (var item in configurator.Shipments)
        {
            _output.WriteLine(item.Options.PackagingType);
        }

        Assert.Equal(count, configurator.Shipments.Count);
    }

    [Theory]
    [InlineData(8.0, 4.0, 0.75, 15.99, 4)]
    public void Check_SetShipment_For_Envelope(decimal l, decimal w, decimal h, decimal oz, int count)
    {
        var destinationAddress = new Address(
                streetLine: "512 Venture Place",
                city: "Barrhead",
                stateOrProvince: "AB",
                postalCode: "T0G 0E0",
                countryCode: "CA");

        var weight = new PackageWeight(oz, isOunce: true);

        var packages = new List<Package>
        {
            new Package(
            new Dimensions()
            {
                Length = l,
                Width = w,
                Height = h,
            },
            weight.InPounds),
        };

        var configurator = new StampsRateConfigurator(_originAddress, destinationAddress, packages[0]);
        configurator.Shipments.Clear();

        configurator.SetShipment(StampsPackageType.LargeEnvelopeOrFlat);
        configurator.SetShipment(StampsPackageType.FlatRatePaddedEnvelope);
        configurator.SetShipment(StampsPackageType.ThickEnvelope);
        configurator.SetShipment(StampsPackageType.LargeEnvelopeOrFlat);

        foreach (var item in configurator.Shipments)
        {
            _output.WriteLine(item.Options.PackagingType);
        }

        Assert.Equal(count, configurator.Shipments.Count);
    }
}
