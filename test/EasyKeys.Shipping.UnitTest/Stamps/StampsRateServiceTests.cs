using System.Collections;

using Bet.Extensions.Testing.Logging;

using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Services;
using EasyKeys.Shipping.Stamps.Abstractions.Services.Impl;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Moq;

namespace EasyKeysShipping.UnitTest.Stamps
{
    public class StampsRateServiceTests
    {
        private readonly ITestOutputHelper _output;
        private readonly IRatesService _ratesService;

        public StampsRateServiceTests(ITestOutputHelper output)
        {
            _output = output;
            _ratesService = GetServices().GetRequiredService<IRatesService>();
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

            Assert.NotNull(domesticRates);

            Assert.True(domesticRates.All(x => x.ContentType == returnedContentType));
        }

        [Theory]
        [ClassData(typeof(CarrierTypeData))]
        public async Task Return_RatesV40_Carrier_Successfully(string carrier, int ratesReturnedCount)
        {
            var rateRequest = new RateRequestDetails() { Carrier = carrier };

            var domesticShipment = CreateDomesticShipment();

            var domesticRates = await _ratesService.GetRatesResponseAsync(domesticShipment, rateRequest, CancellationToken.None);

            Assert.NotNull(domesticRates);

            // service will only return rates for "usps"
            Assert.True(carrier.Contains("usps") || carrier.Contains("default") ? domesticRates.Count > ratesReturnedCount : domesticRates.Count == ratesReturnedCount);
        }

        [Theory]
        [ClassData(typeof(PackageTypeData))]
        public async Task Return_RatesV40_PackageType_Successfully(PackageType packageType, StampsClient.v111.PackageTypeV11 stampsPackageType)
        {
            var rateRequest = new RateRequestDetails() { PackageType = packageType };

            var domesticShipment = CreateDomesticShipment();

            var domesticRates = await _ratesService.GetRatesResponseAsync(domesticShipment, rateRequest, CancellationToken.None);

            Assert.NotNull(domesticRates);

            Assert.True(domesticRates.All(x => x.PackageType == stampsPackageType));
        }

        /// <summary>
        /// This test simulates a <RequriesAllof></RequriesAllof> object and tests to make sure at least one of the
        /// AddOnTypes are added from this list.
        ///
        /// Collection of one or more <requiresoneof> objects.
        /// This value is a hint to the integration that there are required add-ons to go with this add-on.
        /// If this add-on is selected, the integration must also choose exactly one add-on from each set of add-ons listed in the <requiresallof> element in order to form a valid rate to be passed to CreateIndicium.
        /// The integration may use this hint in preparing a user interface with pre-validation for its users.
        /// <see cref="file:///C:/Users/ucren/source/repos/EasyKeys.Shipping/src/EasyKeys.Shipping.Stamps.Abstractions/wsdls/SWS%20-%20Developer%20Guide%20v1.0.pdf">see docs # 1</see>
        /// <see href="https://developer.stamps.com/soap-api/reference/swsimv111.html#cleanseaddressresponse-object">see docs # 2</see>
        /// <returns></returns>
        [Theory]
        [ClassData(typeof(AddOnTypeData))]
        public async Task Return_RatesV40_AddOns_Successfully(StampsClient.v111.AddOnTypeV17 addOnTypeV17)
        {
            /*
             * Data type of RequriesAllOf = AddOnTypeV17[][]
            < RequiresAllOf >
                < RequiresOneOf >
                    < AddOnTypeV17 > US - A - COD </ AddOnTypeV17 >
                    < AddOnTypeV17 > US - A - REG </ AddOnTypeV17 >
                    < AddOnTypeV17 > US - A - CM </ AddOnTypeV17 >
                    < AddOnTypeV17 > US - A - INS </ AddOnTypeV17 >
                    < AddOnTypeV17 > US - A - ASR </ AddOnTypeV17 >
                    < AddOnTypeV17 > US - A - ASRD </ AddOnTypeV17 >
                    < AddOnTypeV17 > US - A - SC </ AddOnTypeV17 >
                </ RequiresOneOf >
            </ RequiresAllOf >
            */
            var getRatesResponse = new StampsClient.v111.GetRatesResponse()
            {
                Rates = new StampsClient.v111.RateV40[1]
                {
                    new StampsClient.v111.RateV40()
                    {
                        RequiresAllOf = new StampsClient.v111.AddOnTypeV17[5][]
                    }
                }
            };

            getRatesResponse.Rates.FirstOrDefault().RequiresAllOf.Append(new StampsClient.v111.AddOnTypeV17[5]);

            for (var i = 0; i < getRatesResponse.Rates.FirstOrDefault().RequiresAllOf.Count(); i++)
            {
                getRatesResponse.Rates.FirstOrDefault().RequiresAllOf[i] = new StampsClient.v111.AddOnTypeV17[2]
                {
                    addOnTypeV17,
                    StampsClient.v111.AddOnTypeV17.USACOM
                };
            }

            var domesticShipment = CreateDomesticShipment();

            var rateRequest = new RateRequestDetails();
            var stampsClientMock = new Mock<IStampsClientService>();

            var swsimV111SoapClientMock = new Mock<StampsClient.v111.SwsimV111Soap>();

            swsimV111SoapClientMock.Setup(x => x.GetRatesAsync(It.IsAny<StampsClient.v111.GetRatesRequest>()))
                .ReturnsAsync(getRatesResponse);

            stampsClientMock.Setup(x => x.CreateClient())
                .Returns(swsimV111SoapClientMock.Object);

            var ratesService = new RatesService(stampsClientMock.Object);

            var response = await ratesService.GetRatesResponseAsync(domesticShipment, rateRequest, CancellationToken.None);

            Assert.NotNull(response);

            Assert.True(response.All(x => x.AddOns.Any(x => x.AddOnType == addOnTypeV17)));
        }

        /// <summary>
        /// Packages dimensions are small enough to fit all service types.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Packages dimensions are small enough to fit all service types.
        /// </summary>
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
            services.AddStampsClient();
            services.AddStampsRateProvider();
            return services.BuildServiceProvider();
            //services.BuildServiceProvider().GetRequiredService<IOptions<StampsOptions>>();
            //return services.BuildServiceProvider().GetRequiredService<IRatesService>();
        }

        public class AddOnTypeData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { StampsClient.v111.AddOnTypeV17.USADC };

                yield return new object[] { StampsClient.v111.AddOnTypeV17.CARADSR };

                yield return new object[] { StampsClient.v111.AddOnTypeV17.USAPR };

                yield return new object[] { StampsClient.v111.AddOnTypeV17.CARANSP };

                yield return new object[] { StampsClient.v111.AddOnTypeV17.SCAINS };
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        /// <summary>
        /// PackageType.Unkown sends back all possible packageTypes, if unknown will default to PackageType.Package
        /// </summary>
        public class PackageTypeData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { PackageType.Pak, StampsClient.v111.PackageTypeV11.Pak };

                yield return new object[] { PackageType.Package, StampsClient.v111.PackageTypeV11.Package };

                yield return new object[] { PackageType.Oversized_Package, StampsClient.v111.PackageTypeV11.OversizedPackage };

                yield return new object[] { PackageType.Large_Package, StampsClient.v111.PackageTypeV11.LargePackage };

                yield return new object[] { PackageType.PostCard, StampsClient.v111.PackageTypeV11.Postcard };

                yield return new object[] { PackageType.Documents, StampsClient.v111.PackageTypeV11.Documents };

                yield return new object[] { PackageType.Thick_Envelope, StampsClient.v111.PackageTypeV11.ThickEnvelope };

                yield return new object[] { PackageType.Envelope, StampsClient.v111.PackageTypeV11.Envelope };

                yield return new object[] { PackageType.Express_Envelope, StampsClient.v111.PackageTypeV11.ExpressEnvelope };

                yield return new object[] { PackageType.Flat_Rate_Envelope, StampsClient.v111.PackageTypeV11.FlatRateEnvelope };

                yield return new object[] { PackageType.Legal_Flat_Rate_Envelope, StampsClient.v111.PackageTypeV11.LegalFlatRateEnvelope };

                yield return new object[] { PackageType.Unknown, StampsClient.v111.PackageTypeV11.Package };

                yield return new object[] { PackageType.Letter, StampsClient.v111.PackageTypeV11.Letter };

                yield return new object[] { PackageType.Large_Envelope_Or_Flat, StampsClient.v111.PackageTypeV11.LargeEnvelopeorFlat };

                yield return new object[] { PackageType.Small_Flat_Rate_Box, StampsClient.v111.PackageTypeV11.SmallFlatRateBox };

                yield return new object[] { PackageType.Flat_Rate_Box, StampsClient.v111.PackageTypeV11.FlatRateBox };

                yield return new object[] { PackageType.Large_Flat_Rate_Box, StampsClient.v111.PackageTypeV11.LargeFlatRateBox };

                yield return new object[] { PackageType.Flat_Rate_Padded_Envelope, StampsClient.v111.PackageTypeV11.FlatRatePaddedEnvelope };

                yield return new object[] { PackageType.Regional_Rate_Box_A, StampsClient.v111.PackageTypeV11.RegionalRateBoxA };

                yield return new object[] { PackageType.Regional_Rate_Box_B, StampsClient.v111.PackageTypeV11.RegionalRateBoxB };

                yield return new object[] { PackageType.Regional_Rate_Box_C, StampsClient.v111.PackageTypeV11.RegionalRateBoxC };
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public class CarrierTypeData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { "usps", 1 };

                yield return new object[] { "ups", 0 };

                yield return new object[] { "dhlexpress", 0 };

                yield return new object[] { "fedex", 0 };

                yield return new object[] { "default", 1 };
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
