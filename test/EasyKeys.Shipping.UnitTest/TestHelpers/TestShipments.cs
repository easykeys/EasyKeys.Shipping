using EasyKeys.Shipping.Abstractions.Models;

namespace EasyKeysShipping.UnitTest.TestHelpers
{
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
                    Height = 20.00M,
                    Width = 15.00M,
                    Length = 12.00M
                },
                .5M),
        };

            var sender = new ContactInfo()
            {
                FirstName = "Brandon",
                LastName = "Moffett",
                Company = "EasyKeys.com",
                Email = "TestMe@EasyKeys.com",
                Department = "Software",
                PhoneNumber = "951-223-2222"
            };
            var receiver = new ContactInfo()
            {
                FirstName = "Fictitious Character",
                Company = "Marvel",
                Email = "FictitiousCharacter@marvel.com",
                Department = "SuperHero",
                PhoneNumber = "867-338-2737"
            };

            var validatedAddress = new ValidateAddress(Guid.NewGuid().ToString(), destinationAddress);

            return new Shipment(originAddress, validatedAddress.ProposedAddress ?? validatedAddress.OriginalAddress, packages)
            {
                RecipientInfo = receiver,
                SenderInfo = sender,
            };
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

            var packages = new List<Package>
        {
            new Package(
                new Dimensions()
                {
                    Height = 2.00M,
                    Width = 1.500M,
                    Length = 1.200M
                },
                .5M),
        };

            var commodity = new Commodity()
            {
                Description = "ekjs",
                CountryOfManufacturer = "US",
                PartNumber = "kjsdf",
                Amount = 10m,
                CustomsValue = 1m,
                NumberOfPieces = 1,
                Quantity = 1,
                ExportLicenseNumber = "dsdfs",
                Name = "sdkfsdf",
                Weight = .3m
            };

            var sender = new ContactInfo()
            {
                FirstName = "Brandon",
                LastName = "Moffett",
                Company = "EasyKeys.com",
                Email = "TestMe@EasyKeys.com",
                Department = "Software",
                PhoneNumber = "951-223-2222"
            };
            var receiver = new ContactInfo()
            {
                FirstName = "Fictitious Character",
                Company = "Marvel",
                Email = "FictitiousCharacter@marvel.com",
                Department = "SuperHero",
                PhoneNumber = "867-338-2737"
            };

            var validatedAddress = new ValidateAddress(Guid.NewGuid().ToString(), destinationAddress);

            var shipment = new Shipment(originAddress, validatedAddress.ProposedAddress ?? validatedAddress.OriginalAddress, packages)
            {
                RecipientInfo = receiver,
                SenderInfo = sender
            };
            shipment.Commodities.Add(commodity);

            return shipment;
        }
    }
}
