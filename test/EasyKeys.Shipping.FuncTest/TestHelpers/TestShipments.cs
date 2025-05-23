using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Models;

namespace EasyKeysShipping.FuncTest.TestHelpers;

public static class TestShipments
{
    /// <summary>
    /// Packages dimensions are small enough to fit all service types.
    /// </summary>
    /// <returns></returns>
    public static Shipment CreateDomesticShipment()
    {
        var originAddress = new Address(
            streetLine: "11407 Granite Street",
            city: "Charlotte",
            stateOrProvince: "NC",
            postalCode: "28273",
            countryCode: "US");

        var destinationAddress = new Address(
            streetLine: "1550 central avenue",
            city: "riverside",
            stateOrProvince: "CA",
            postalCode: "92507",
            countryCode: "US");

        var packages = new List<Package>
        {
            new Package(
                new Dimensions()
                {
                    Height = 9.00M,
                    Width = 6.00M,
                    Length = 1.00M
                },
                .0625M),
        };

        var validatedAddress = new ValidateAddress(Guid.NewGuid().ToString(), destinationAddress);
        var shipmentOptions = new ShipmentOptions(StampsPackageType.Package.Name, DateTime.Now.AddDays(1));
        return new Shipment(
            originAddress,
            validatedAddress.ProposedAddress ?? validatedAddress.OriginalAddress,
            packages,
            shipmentOptions);
    }

    /// <summary>
    /// Packages dimensions are small enough to fit all service types.
    /// </summary>
    public static Shipment CreateInternationalShipment()
    {
        var originAddress = new Address(
            streetLine: "11407 Granite Street",
            city: "Charlotte",
            stateOrProvince: "NC",
            postalCode: "28273",
            countryCode: "US");

        var destinationAddress = new Address(
            streetLine: "24 Sussex Drive",
            city: "Ottawa",
            stateOrProvince: "ON",
            postalCode: "K1M 1M4",
            countryCode: "CA");
        destinationAddress = new Address("285 Wang Fu Jing Avenue", "BEIJING", "", "100006", "CN", isResidential: false);
        var packages = new List<Package>
        {
            new Package(
                new Dimensions()
                {
                    Height = 2.00M,
                    Width = 1.500M,
                    Length = 1.200M
                },
                .0625m,
                10m,
                false),
        };
        var validatedAddress = new ValidateAddress(Guid.NewGuid().ToString(), destinationAddress);
        var shipmentOptions = new ShipmentOptions(StampsPackageType.Package.Name, DateTime.Now);

        var shipment = new Shipment(
            originAddress,
            validatedAddress.ProposedAddress ?? validatedAddress.OriginalAddress,
            packages,
            shipmentOptions);

        return shipment;
    }

    public static (ContactInfo, ContactInfo) CreateContactInfo()
    {
        return (
            new ContactInfo()
            {
                FirstName = "Brandon",
                LastName = "Moffett",
                Company = "EasyKeys.com",
                Email = "TestMe@EasyKeys.com",
                PhoneNumber = "951-223-2222",
                TaxId = "26-1234567"
            },
            new ContactInfo()
            {
                FirstName = "Fictitious Character",
                Company = "Marvel",
                Email = "FictitiousCharacter@marvel.com",
                PhoneNumber = "867-338-2737"
            });
    }
}
