using System.Collections;
using System.ServiceModel;

using Bet.Extensions.Testing.Logging;

using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Options;
using EasyKeys.Shipping.Stamps.Abstractions.Services;
using EasyKeys.Shipping.Stamps.Rates;
using EasyKeys.Shipping.Stamps.Rates.Models;

using EasyKeysShipping.UnitTest.Stubs;
using EasyKeysShipping.UnitTest.TestHelpers;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

using Moq;

using StampsClient.v111;

namespace EasyKeysShipping.UnitTest.Stamps;

public class StampsRateServiceTests
{
    private readonly ITestOutputHelper _output;
    private readonly IStampsRateProvider _ratesProvider;

    public StampsRateServiceTests(ITestOutputHelper output)
    {
        _output = output;
        _ratesProvider = GetServices().GetRequiredService<IStampsRateProvider>();
    }

    [Fact]
    public async Task Return_RatesV40_Address_Successfully()
    {
        var internationalShipment = TestShipments.CreateInternationalShipment();

        var domesticShipment = TestShipments.CreateDomesticShipment();

        var internationalRates = await _ratesProvider.GetInternationalRatesAsync(internationalShipment, new RateInternationalOptions(), CancellationToken.None);

        var domesticRates = await _ratesProvider.GetDomesticRatesAsync(domesticShipment, new RateOptions(), CancellationToken.None);

        // check ToAddress Province/State & PostalCode/ZipCode logic
        Assert.NotNull(domesticRates);

        Assert.True(domesticRates.Rates.Count > 0);

        Assert.NotNull(internationalRates);

        Assert.True(internationalRates.Rates.Count > 0);
    }

    [Theory]
    [ClassData(typeof(ServiceTypeData))]
    public async Task Return_RatesV40_ServiceType_Successfully(StampsServiceType serviceType, ServiceType returnServiceType)
    {
        var rateDomesticOptions = new RateOptions()
        {
            ServiceType = serviceType
        };

        var domesticShipment = TestShipments.CreateDomesticShipment();

        var internationalShipment = TestShipments.CreateInternationalShipment();

        var rateIntlOptions = new RateInternationalOptions()
        {
            ServiceType = serviceType
        };

        // Mail class UspsReturn not supported.
        var result = serviceType.Description.Contains("International", StringComparison.OrdinalIgnoreCase) ? await _ratesProvider.GetInternationalRatesAsync(internationalShipment, rateIntlOptions, CancellationToken.None)
            : await _ratesProvider.GetDomesticRatesAsync(domesticShipment, rateDomesticOptions, CancellationToken.None);

        // check ToAddress Province/State & PostalCode/ZipCode logic
        Assert.NotNull(result);

        if (serviceType == StampsServiceType.Unknown)
        {
            // when unknown, defaults to all available
            Assert.True(result.Rates.Any());
        }
        else
        {
            Assert.True(result.Rates.All(x => x.ServiceName == serviceType.Name));
        }
    }

    [Theory]
    [ClassData(typeof(CarrierTypeData))]
    public async Task Return_RatesV40_Carrier_Successfully(CarrierType carrier, int ratesReturnedCount)
    {
        var rateRequest = new RateOptions()
        {
            Carrier = carrier
        };

        var domesticShipment = TestShipments.CreateDomesticShipment();

        var domesticRates = await _ratesProvider.GetDomesticRatesAsync(domesticShipment, rateRequest, CancellationToken.None);

        Assert.NotNull(domesticRates);

        // service will only return rates for "usps"
        Assert.True(carrier.Name.Contains("usps", StringComparison.OrdinalIgnoreCase) ? domesticRates.Rates.Count > ratesReturnedCount : domesticRates.Rates.Count == ratesReturnedCount);
    }

    [Theory]
    [ClassData(typeof(PackageTypeData))]
    public async Task Return_RatesV40_PackageType_Successfully(PackageType packageType, PackageTypeV11 stampsPackageType)
    {
        var rateRequest = new RateOptions();

        var domesticShipment = TestShipments.CreateDomesticShipment();

        var shipment = new Shipment(domesticShipment.OriginAddress, domesticShipment.DestinationAddress, domesticShipment.Packages, new ShipmentOptions(packageType.Name, DateTime.Now));

        var domesticRates = await _ratesProvider.GetDomesticRatesAsync(shipment, rateRequest, CancellationToken.None);

        Assert.NotNull(domesticRates);

        Assert.True(domesticRates.Rates.All(x => x.PackageType == packageType.Name));
    }

    /// <summary>
    /// This test simulates a <RequriesAllof></RequriesAllof> object and tests to make sure at least one of the
    /// AddOnTypes are added from this list.
    ///
    /// Collection of one or more. <requiresoneof> objects.
    /// This value is a hint to the integration that there are required add-ons to go with this add-on.
    /// If this add-on is selected, the integration must also choose exactly one add-on from each set of add-ons listed in the <requiresallof> element in order to form a valid rate to be passed to CreateIndicium.
    /// The integration may use this hint in preparing a user interface with pre-validation for its users.
    /// <see cref="file:///C:/Users/ucren/source/repos/EasyKeys.Shipping/src/EasyKeys.Shipping.Stamps.Abstractions/wsdls/SWS%20-%20Developer%20Guide%20v1.0.pdf">see docs # 1</see>
    /// <see href="https://developer.stamps.com/soap-api/reference/swsimv111.html#cleanseaddressresponse-object">see docs # 2</see>
    /// <returns></returns>
    [Theory]
    [ClassData(typeof(AddOnTypeData))]
    public async Task Return_RatesV40_AddOns_Successfully(AddOnTypeV17 addOnTypeV17)
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
        var getRatesResponse = new GetRatesResponse()
        {
            Rates = new RateV40[1]
            {
                new RateV40()
                {
                    RequiresAllOf = new AddOnTypeV17[5][]
                }
            }
        };

        getRatesResponse.Rates.FirstOrDefault().RequiresAllOf.Append(new AddOnTypeV17[5]);

        for (var i = 0; i < getRatesResponse.Rates.FirstOrDefault().RequiresAllOf.Count(); i++)
        {
            getRatesResponse.Rates.FirstOrDefault().RequiresAllOf[i] = new AddOnTypeV17[2]
            {
                addOnTypeV17,
                AddOnTypeV17.USACOM
            };
        }

        var domesticShipment = TestShipments.CreateDomesticShipment();

        var rateOptions = new RateOptions();
        var mockOptions = new Mock<IOptionsMonitor<StampsOptions>>();
        var mockAuth = new Mock<IStampsClientAuthenticator>();
        var loggerFactory = new NullLoggerFactory();
        var mockLogger = new Mock<ILogger<StampsClientServiceMock>>();
        var mockSoapClient = new Mock<SwsimV111Soap>();

        mockOptions.Setup(x => x.CurrentValue).Returns(new StampsOptions());
        mockAuth.Setup(x => x.GetToken()).Returns("testing");

        mockSoapClient.Setup(x => x.GetRatesAsync(It.IsAny<GetRatesRequest>()))
            .ReturnsAsync(getRatesResponse);

        var stampsClient = new StampsClientServiceMock(
            mockOptions.Object,
            mockAuth.Object,
            mockSoapClient.Object,
            loggerFactory,
            mockLogger.Object);

        var rateProviderLogger = new Mock<ILogger<StampsRateProvider>>();

        var ratesService = new StampsRateProvider(stampsClient, rateProviderLogger.Object);

        var response = await ratesService.GetDomesticRatesAsync(domesticShipment, rateOptions, CancellationToken.None);

        Assert.NotNull(response);

        // Assert.True(response.Rates.All(x => x.AddOns.Any(x => x.AddOnType == addOnTypeV17)));
    }

    [Theory]
    [InlineData("Conversation out-of-sync.")]
    [InlineData("Invalid conversation token.")]
    [InlineData("Authentication failed.")]
    public async void RateService_Handles_Exceptions_Successfully(string exMessage)
    {
        // arrange
        var domesticShipment = TestShipments.CreateDomesticShipment();

        var rateOptions = new RateOptions();

        var mockOptions = new Mock<IOptionsMonitor<StampsOptions>>();
        var mockAuth = new Mock<IStampsClientAuthenticator>();
        var loggerFactory = new NullLoggerFactory();
        var mockLogger = new Mock<ILogger<StampsClientServiceMock>>();
        var mockSoapClient = new Mock<SwsimV111Soap>();

        mockOptions.Setup(x => x.CurrentValue).Returns(new StampsOptions());
        mockAuth.Setup(x => x.GetToken()).Returns("testing");
        mockSoapClient.Setup(x => x.GetRatesAsync(It.IsAny<GetRatesRequest>()))
            .Verifiable();

        mockSoapClient.SetupSequence(x => x.GetRatesAsync(It.IsAny<GetRatesRequest>()))
            .ThrowsAsync(new FaultException(exMessage))
            .ThrowsAsync(new Exception(exMessage));

        var stampsClient = new StampsClientServiceMock(
            mockOptions.Object,
            mockAuth.Object,
            mockSoapClient.Object,
            loggerFactory,
            mockLogger.Object);

        var rateProviderLogger = new Mock<ILogger<StampsRateProvider>>();

        var stampsAddressValidationProvider = new StampsRateProvider(stampsClient, rateProviderLogger.Object);

        // act - assert
        var ex = await Assert.ThrowsAsync<Exception>(async () => await stampsAddressValidationProvider.GetDomesticRatesAsync(domesticShipment, rateOptions, CancellationToken.None));

        Assert.True(ex.Message == exMessage);

        mockSoapClient.Verify(x => x.GetRatesAsync(It.IsAny<GetRatesRequest>()), Times.Exactly(2));
    }

    [Theory]
    [InlineData("Conversation out-of-sync.")]
    public async void RateService_Refreshes_Token_And_Returns_Rates_Successfully(string exMessage)
    {
        // arrange
        var domesticShipment = TestShipments.CreateDomesticShipment();

        var rateOptions = new RateOptions();

        var mockOptions = new Mock<IOptionsMonitor<StampsOptions>>();
        var mockAuth = new Mock<IStampsClientAuthenticator>();
        var loggerFactory = new NullLoggerFactory();
        var mockLogger = new Mock<ILogger<StampsClientServiceMock>>();
        var mockSoapClient = new Mock<SwsimV111Soap>();

        mockOptions.Setup(x => x.CurrentValue).Returns(new StampsOptions());
        mockAuth.Setup(x => x.GetToken()).Returns("testing");
        mockAuth.Setup(x => x.ClearTokens()).Verifiable();
        mockSoapClient.Setup(x => x.GetRatesAsync(It.IsAny<GetRatesRequest>()))
            .Verifiable();

        mockSoapClient.SetupSequence(x => x.GetRatesAsync(It.IsAny<GetRatesRequest>()))
            .ThrowsAsync(new FaultException(exMessage))
            .ReturnsAsync(new GetRatesResponse() { Authenticator = "test", Rates = new RateV40[0] });

        var stampsClient = new StampsClientServiceMock(
            mockOptions.Object,
            mockAuth.Object,
            mockSoapClient.Object,
            loggerFactory,
            mockLogger.Object);

        var rateProviderLogger = new Mock<ILogger<StampsRateProvider>>();

        var stampsAddressValidationProvider = new StampsRateProvider(stampsClient, rateProviderLogger.Object);

        // act
        var result = await stampsAddressValidationProvider.GetDomesticRatesAsync(domesticShipment, rateOptions, CancellationToken.None);

        // assert
        Assert.IsType<List<RateV40>>(result);
        mockAuth.Verify(x => x.ClearTokens(), Times.Once);
        mockSoapClient.Verify(x => x.GetRatesAsync(It.IsAny<GetRatesRequest>()), Times.Exactly(2));
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
    }

    public class AddOnTypeData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { AddOnTypeV17.USADC };

            yield return new object[] { AddOnTypeV17.CARADSR };

            yield return new object[] { AddOnTypeV17.USAPR };

            yield return new object[] { AddOnTypeV17.CARANSP };

            yield return new object[] { AddOnTypeV17.SCAINS };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    /// <summary>
    /// PackageType.Unknown sends back all possible packageTypes, if unknown will default to PackageType.Package.
    /// </summary>
    public class PackageTypeData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { PackageType.Pak, PackageTypeV11.Pak };

            yield return new object[] { PackageType.Package, PackageTypeV11.Package };

            yield return new object[] { PackageType.OversizedPackage, PackageTypeV11.OversizedPackage };

            yield return new object[] { PackageType.LargePackage, PackageTypeV11.LargePackage };

            yield return new object[] { PackageType.PostCard, PackageTypeV11.Postcard };

            yield return new object[] { PackageType.Documents, PackageTypeV11.Documents };

            yield return new object[] { PackageType.ThickEnvelope, PackageTypeV11.ThickEnvelope };

            yield return new object[] { PackageType.Envelope, PackageTypeV11.Envelope };

            yield return new object[] { PackageType.ExpressEnvelope, PackageTypeV11.ExpressEnvelope };

            yield return new object[] { PackageType.FlatRateEnvelope, PackageTypeV11.FlatRateEnvelope };

            yield return new object[] { PackageType.LegalFlatRateEnvelope, PackageTypeV11.LegalFlatRateEnvelope };

            yield return new object[] { PackageType.Letter, PackageTypeV11.Letter };

            yield return new object[] { PackageType.LargeEnvelopeOrFlat, PackageTypeV11.LargeEnvelopeorFlat };

            yield return new object[] { PackageType.SmallFlatRateBox, PackageTypeV11.SmallFlatRateBox };

            yield return new object[] { PackageType.FlatRateBox, PackageTypeV11.FlatRateBox };

            yield return new object[] { PackageType.LargeFlatRateBox, PackageTypeV11.LargeFlatRateBox };

            yield return new object[] { PackageType.FlatRatePaddedEnvelope, PackageTypeV11.FlatRatePaddedEnvelope };

            yield return new object[] { PackageType.RegionalRateBoxA, PackageTypeV11.RegionalRateBoxA };

            yield return new object[] { PackageType.RegionalRateBoxB, PackageTypeV11.RegionalRateBoxB };

            yield return new object[] { PackageType.RegionalRateBoxC, PackageTypeV11.RegionalRateBoxC };
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
            yield return new object[] { CarrierType.Usps, 1 };

            yield return new object[] { CarrierType.Ups, 0 };

            yield return new object[] { CarrierType.DhlExpress, 0 };

            yield return new object[] { CarrierType.FedEx, 0 };
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
            yield return new object[] { StampsServiceType.ParcelSelectGround, ServiceType.USPS };

            /* Mailpiece is over maximum weight of 0 lb 15.999 oz
             * Cannot ship First-Class packages larger than 22" x 18" x 15"
             */
            yield return new object[] { StampsServiceType.FirstClass, ServiceType.USFC };

            yield return new object[] { StampsServiceType.MediaMail, ServiceType.USMM };

            yield return new object[] { StampsServiceType.Priority, ServiceType.USPM };

            yield return new object[] { StampsServiceType.PriorityExpress, ServiceType.USXM };

            // Mail class 'ExpressMailInternational' is not available for the destination country.
            yield return new object[] { StampsServiceType.PriorityExpressInternational, ServiceType.USEMI };

            // only for international
            yield return new object[] { StampsServiceType.FirstClassInternational, ServiceType.USFCI };

            // Mail class UspsReturn not supported.
            // yield return new object[] { ServiceType.USPS_PAY_ON_USE_RETURN, StampsClient.v111.ServiceType.USRETURN };
            yield return new object[] { StampsServiceType.LibraryMail, ServiceType.USLM };

            // Mail class 'PriorityMailInternational' is not available for the destination country.
            yield return new object[] { StampsServiceType.PriorityInternational, ServiceType.USPMI };

            // when unknown, defaults to all available
            yield return new object[] { StampsServiceType.Unknown, ServiceType.USPM };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
