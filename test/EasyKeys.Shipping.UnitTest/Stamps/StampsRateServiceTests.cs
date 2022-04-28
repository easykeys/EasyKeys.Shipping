
using System.Collections;

using Bet.Extensions.Testing.Logging;

using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EasyKeysShipping.UnitTest.Stamps
{
    public class StampsRateServiceTests
    {
        private readonly ITestOutputHelper _output;
        private readonly IRatesService _ratesService;

        public StampsRateServiceTests(ITestOutputHelper output)
        {
            _output = output;
            _ratesService = GetServices();
        }

        [Fact]
        public async Task Return_RatesV40_Address_Successfully()
        {
            var rateRequest = new RateRequestDetails();

            var internationalShipment = CreateInternationalShipment();

            var domesticShipment = CreateDomesticShipment();

            var internationalRates = await _ratesService.GetRatesResponseAsync(internationalShipment, rateRequest, CancellationToken.None);

            var domesticRates = await _ratesService.GetRatesResponseAsync(domesticShipment, rateRequest, CancellationToken.None);

            // check ToAddress Province/State & PostalCode/ZipCode logic
            Assert.NotNull(domesticRates);

            Assert.True(domesticRates.Count() > 0);

            Assert.True(domesticRates.All(x => x.To.State == domesticShipment.DestinationAddress.StateOrProvince));

            Assert.True(domesticRates.All(x => x.To.ZIPCode == domesticShipment.DestinationAddress.PostalCode));

            Assert.NotNull(internationalRates);

            Assert.True(internationalRates.Count() > 0);

            Assert.True(internationalRates.All(x => x.To.Province == internationalShipment.DestinationAddress.StateOrProvince));

            Assert.True(internationalRates.All(x => x.To.PostalCode == internationalShipment.DestinationAddress.PostalCode));
        }

        [Theory]
        [ClassData(typeof(ServiceTypeData))]
        public async Task Return_RatesV40_ServiceType_Successfully(ServiceType serviceType, StampsClient.v111.ServiceType returnServiceType)
        {
            var rateRequest = new RateRequestDetails()
            {
                ServiceType = serviceType
            };

            var domesticShipment = CreateDomesticShipment();

            var internationalShipment = CreateInternationalShipment();
            // Mail class UspsReturn not supported.

            var rates = serviceType.ToString().Contains("international", StringComparison.OrdinalIgnoreCase) ? await _ratesService.GetRatesResponseAsync(internationalShipment, rateRequest, CancellationToken.None)
                : await _ratesService.GetRatesResponseAsync(domesticShipment, rateRequest, CancellationToken.None);

            // check ToAddress Province/State & PostalCode/ZipCode logic
            Assert.NotNull(rates);

            if (serviceType == ServiceType.UNKNOWN)
            {
                // when unknown, defaults to all available
                Assert.True(rates.Any());
            }
            else
            {
                Assert.True(rates.All(x => x.ServiceType == returnServiceType));
            }
        }

        [Theory]
        [ClassData(typeof(ContentTypeData))]
        public async Task Return_RatesV40_ContentType_Successfully(string contentType, StampsClient.v111.ContentTypeV2 returnedContentType)
        {
            var rateRequest = new RateRequestDetails() { ContentType = contentType };

            var domesticShipment = CreateDomesticShipment();

            var domesticRates = await _ratesService.GetRatesResponseAsync(domesticShipment, rateRequest, CancellationToken.None);

            // check ToAddress Province/State & PostalCode/ZipCode logic
            Assert.NotNull(domesticRates);

            Assert.True(domesticRates.All(x => x.ContentType == returnedContentType));
        }

        [Theory]
        [ClassData(typeof(CarrierTypeData))]
        public async Task Return_RatesV40_Carrier_Successfully(string carrier, StampsClient.v111.Carrier returnedCarrierType)
        {
            var rateRequest = new RateRequestDetails() { Carrier = carrier };

            var domesticShipment = CreateDomesticShipment();

            var domesticRates = await _ratesService.GetRatesResponseAsync(domesticShipment, rateRequest, CancellationToken.None);

            // check ToAddress Province/State & PostalCode/ZipCode logic
            Assert.NotNull(domesticRates);

            // Assert.True(domesticRates.All(x => x.c == returnedCarrierType));
        }

        private Shipment CreateDomesticShipment()
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

        private Shipment CreateInternationalShipment()
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
                Weight = 13m
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

        private IRatesService GetServices()
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
            services.AddStampsRateProvider();

            return services.BuildServiceProvider().GetRequiredService<IRatesService>();
        }

        public class CarrierTypeData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { "usps", StampsClient.v111.Carrier.USPS };

                yield return new object[] { "ups", StampsClient.v111.Carrier.UPS };

                yield return new object[] { "dhlexpress", StampsClient.v111.Carrier.DHLExpress };

                yield return new object[] { "fedex", StampsClient.v111.Carrier.FedEx };

                yield return new object[] { "default", StampsClient.v111.Carrier.USPS };
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public class ServiceTypeData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { ServiceType.USPS_PARCEL_SELECT_GROUND, StampsClient.v111.ServiceType.USPS };

                /* Mailpiece is over maximum weight of 0 lb 15.999 oz
                 * Cannot ship First-Class packages larger than 22" x 18" x 15"
                 */
                yield return new object[] { ServiceType.USPS_FIRST_CLASS_MAIL, StampsClient.v111.ServiceType.USFC };

                yield return new object[] { ServiceType.USPS_MEDIA_MAIL, StampsClient.v111.ServiceType.USMM };

                yield return new object[] { ServiceType.USPS_PRIORITY_MAIL, StampsClient.v111.ServiceType.USPM };

                yield return new object[] { ServiceType.USPS_PRIORITY_MAIL_EXPRESS, StampsClient.v111.ServiceType.USXM };

                // Mail class 'ExpressMailInternational' is not available for the destination country.
                yield return new object[] { ServiceType.USPS_PRIORITY_MAIL_EXPRESS_INTERNATIONAL, StampsClient.v111.ServiceType.USEMI };

                // only for international
                yield return new object[] { ServiceType.USPS_FIRST_CLASS_MAIL_INTERNATIONAL, StampsClient.v111.ServiceType.USFCI };

                // Mail class UspsReturn not supported.
                // yield return new object[] { ServiceType.USPS_PAY_ON_USE_RETURN, StampsClient.v111.ServiceType.USRETURN };

                yield return new object[] { ServiceType.USPS_LIBRARY_MAIL, StampsClient.v111.ServiceType.USLM };

                // Mail class 'PriorityMailInternational' is not available for the destination country.
                yield return new object[] { ServiceType.USPS_PRIORITY_MAIL_INTERNATIONAL, StampsClient.v111.ServiceType.USPMI };

                // when unknown, defaults to all available
                yield return new object[] { ServiceType.UNKNOWN, StampsClient.v111.ServiceType.USPM };
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public class ContentTypeData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { "commercial_sample", StampsClient.v111.ContentTypeV2.CommercialSample };

                /* Mailpiece is over maximum weight of 0 lb 15.999 oz
                 * Cannot ship First-Class packages larger than 22" x 18" x 15"
                 */
                yield return new object[] { "dangerous_goods", StampsClient.v111.ContentTypeV2.DangerousGoods };

                yield return new object[] { "document", StampsClient.v111.ContentTypeV2.Document };

                yield return new object[] { "gift", StampsClient.v111.ContentTypeV2.Gift };

                yield return new object[] { "humanitarian", StampsClient.v111.ContentTypeV2.HumanitarianDonation };

                // Mail class 'ExpressMailInternational' is not available for the destination country.
                yield return new object[] { "merchandise", StampsClient.v111.ContentTypeV2.Merchandise };

                // only for international
                yield return new object[] { "returned_goods", StampsClient.v111.ContentTypeV2.ReturnedGoods };

                // Mail class UspsReturn not supported.
                // yield return new object[] { ServiceType.USPS_PAY_ON_USE_RETURN, StampsClient.v111.ServiceType.USRETURN };

                yield return new object[] { "other", StampsClient.v111.ContentTypeV2.Other };
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
