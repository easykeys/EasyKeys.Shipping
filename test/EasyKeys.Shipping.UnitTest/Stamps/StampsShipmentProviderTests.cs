using System.Collections;
using System.ServiceModel;

using Bet.Extensions.Testing.Logging;

using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Options;
using EasyKeys.Shipping.Stamps.Abstractions.Services;
using EasyKeys.Shipping.Stamps.Rates.Models;
using EasyKeys.Shipping.Stamps.Shipment;
using EasyKeys.Shipping.Stamps.Shipment.DependencyInjection;
using EasyKeys.Shipping.Stamps.Shipment.Models;

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

public class StampsShipmentProviderTests
{
    private readonly ITestOutputHelper _output;
    private readonly IStampsShipmentProvider _shipmentProvider;

    public StampsShipmentProviderTests(ITestOutputHelper output)
    {
        _output = output;
        _shipmentProvider = GetShipmentProvider();
    }

    [Fact]
    public async Task Process_Domestic_Shipment_Successfully()
    {
        var rate = new Rate("USPM", string.Empty, string.Empty, 0m, DateTime.Now);

        var (sender, recipient) = TestShipments.CreateContactInfo();

        var shipmentDetails = new ShipmentDetails();

        var rateOptions = new RateOptions()
        {
            Sender = sender,
            Recipient = recipient
        };

        var labels = await _shipmentProvider.CreateDomesticShipmentAsync(
              TestShipments.CreateDomesticShipment(),
              rateOptions,
              shipmentDetails,
              CancellationToken.None);

        Assert.NotNull(labels);
        Assert.NotNull(labels.Labels[0].Bytes[0]);
    }

    [Fact]
    public async Task Process_International_Shipment_Successfully()
    {
        var (sender, recipient) = TestShipments.CreateContactInfo();

        var shipmentDetails = new ShipmentDetails();

        var customsInformation = new CustomsInformation()
        {
            CustomsSigner = "brandon moffett"
        };

        var rateOptions = new RateInternationalOptions()
        {
            Sender = sender,
            Recipient = recipient,
            ServiceType = StampsServiceType.FromName("USPMI")
        };

        var commodities = new List<Commodity>
        {
            new Commodity()
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
            }
        };

        var labels = await _shipmentProvider.CreateInternationalShipmentAsync(
              TestShipments.CreateInternationalShipment(),
              rateOptions,
              shipmentDetails,
              commodities,
              customsInformation,
              CancellationToken.None);

        Assert.NotNull(labels);
        Assert.NotNull(labels.Labels[0].Bytes[0]);
    }

    [Theory]
    [ClassData(typeof(ContentTypeData))]
    public async Task Return_RatesV40_ContentType_Successfully(ContentType contentType, ContentTypeV2 returnedContentType)
    {
        var shipmentDetails = new ShipmentDetails();
        var rateOptions = new RateOptions
        {
            ContentType = contentType
        };

        var domesticShipment = TestShipments.CreateDomesticShipment();

        var createdShipment = await _shipmentProvider.CreateDomesticShipmentAsync(domesticShipment, rateOptions, shipmentDetails, CancellationToken.None);

        Assert.NotNull(createdShipment);
    }

    [Theory]
    [InlineData("Conversation out-of-sync.")]
    [InlineData("Invalid conversation token.")]
    [InlineData("Authentication failed.")]
    public async Task ShipmentProvider_Handles_Exceptions_Successfully(string exMessage)
    {
        // arrange
        var domesticShipment = TestShipments.CreateDomesticShipment();

        var rate = new Rate("USPS", string.Empty, string.Empty, 0m, DateTime.Now) { Name = "USPS" };

        var (sender, recipient) = TestShipments.CreateContactInfo();

        var shipmentDetails = new ShipmentDetails();

        var rateOptions = new RateOptions()
        {
            Sender = sender,
            Recipient = recipient
        };

        var mockOptions = new Mock<IOptionsMonitor<StampsOptions>>();
        var mockAuth = new Mock<IStampsClientAuthenticator>();
        var loggerFactory = new NullLoggerFactory();
        var mockLogger = new Mock<ILogger<StampsClientServiceMock>>();
        var mockSoapClient = new Mock<SwsimV111Soap>();
        var mockLogger2 = new Mock<ILogger<StampsShipmentProvider>>();

        mockOptions.Setup(x => x.CurrentValue).Returns(new StampsOptions());
        mockSoapClient.Setup(x => x.CreateIndiciumAsync(It.IsAny<CreateIndiciumRequest>()))
            .Verifiable();

        mockSoapClient.SetupSequence(x => x.CreateIndiciumAsync(It.IsAny<CreateIndiciumRequest>()))
            .ThrowsAsync(new FaultException(exMessage))
            .ThrowsAsync(new Exception(exMessage));

        var stampsClient = new StampsClientServiceMock(
            mockOptions.Object,
            mockAuth.Object,
            mockSoapClient.Object,
            loggerFactory,
            mockLogger.Object);

        var stampsShipmentProvider = new StampsShipmentProvider(stampsClient, mockLogger2.Object);

        // act
        var result = await stampsShipmentProvider.CreateDomesticShipmentAsync(domesticShipment, rateOptions, shipmentDetails, CancellationToken.None);

        mockSoapClient.Verify(x => x.CreateIndiciumAsync(It.IsAny<CreateIndiciumRequest>()), Times.Exactly(2));

        // assert
        Assert.IsType<ShipmentLabel>(result);

        Assert.Contains(result.InternalErrors, x => x == exMessage);
    }

    [Theory]
    [InlineData("Conversation out-of-sync.")]
    public async Task ShipmentProvider_Refreshes_Token_And_Returns_Shipment_Successfully(string exMessage)
    {
        // arrange
        var rate = new Rate("USPS", string.Empty, string.Empty, 0m, DateTime.Now) { Name = "USPS" };

        var (sender, recipient) = TestShipments.CreateContactInfo();

        var shipmentDetails = new ShipmentDetails();

        var rateOptions = new RateOptions()
        {
            Sender = sender,
            Recipient = recipient
        };

        var domesticShipment = TestShipments.CreateDomesticShipment();

        var mockOptions = new Mock<IOptionsMonitor<StampsOptions>>();
        var mockAuth = new Mock<IStampsClientAuthenticator>();
        var loggerFactory = new NullLoggerFactory();
        var mockLogger = new Mock<ILogger<StampsClientServiceMock>>();
        var mockSoapClient = new Mock<SwsimV111Soap>();
        var mockLogger2 = new Mock<ILogger<StampsShipmentProvider>>();
        mockOptions.Setup(x => x.CurrentValue).Returns(new StampsOptions());
        mockSoapClient.Setup(x => x.CreateIndiciumAsync(It.IsAny<CreateIndiciumRequest>()))
            .Verifiable();

        mockSoapClient.SetupSequence(x => x.CreateIndiciumAsync(It.IsAny<CreateIndiciumRequest>()))
            .ThrowsAsync(new FaultException(exMessage))
            .ReturnsAsync(new CreateIndiciumResponse()
            { Rate = new RateV40(), ImageData = new byte[0][], Authenticator = "test", StampsTxID = Guid.NewGuid(), TrackingNumber = "test" });

        var stampsClient = new StampsClientServiceMock(
            mockOptions.Object,
            mockAuth.Object,
            mockSoapClient.Object,
            loggerFactory,
            mockLogger.Object);

        var stampsShipmentProvider = new StampsShipmentProvider(stampsClient, mockLogger2.Object);

        // act
        var result = await stampsShipmentProvider.CreateDomesticShipmentAsync(domesticShipment, rateOptions, shipmentDetails, CancellationToken.None);

        mockSoapClient.Verify(x => x.CreateIndiciumAsync(It.IsAny<CreateIndiciumRequest>()), Times.Exactly(2));

        // assert
        Assert.IsType<ShipmentLabel>(result);

        Assert.Equal("test", result.Labels.FirstOrDefault().TrackingId);
    }

    private IStampsShipmentProvider GetShipmentProvider()
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
        services.AddStampsShipmentProvider();
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");
        var sp = services.BuildServiceProvider();
        return sp.GetRequiredService<IStampsShipmentProvider>();
    }

    public class ContentTypeData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { ContentType.CommcercialSample, ContentTypeV2.CommercialSample };

            /* Mailpiece is over maximum weight of 0 lb 15.999 oz
             * Cannot ship First-Class packages larger than 22" x 18" x 15"
             */
            yield return new object[] { ContentType.DangerousGoods, ContentTypeV2.DangerousGoods };

            yield return new object[] { ContentType.Document, ContentTypeV2.Document };

            yield return new object[] { ContentType.Gift, ContentTypeV2.Gift };

            yield return new object[] { ContentType.HumanitarianDonation, ContentTypeV2.HumanitarianDonation };

            // Mail class 'ExpressMailInternational' is not available for the destination country.
            yield return new object[] { ContentType.Merchandise, ContentTypeV2.Merchandise };

            // only for international
            yield return new object[] { ContentType.ReturnedGoods, ContentTypeV2.ReturnedGoods };

            // Mail class UspsReturn not supported.
            // yield return new object[] { ServiceType.USPS_PAY_ON_USE_RETURN, StampsClient.v111.ServiceType.USRETURN };

            yield return new object[] { ContentType.Other, ContentTypeV2.Other };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
