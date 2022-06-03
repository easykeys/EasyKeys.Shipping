using System.Collections;
using System.ServiceModel;

using Bet.Extensions.Testing.Logging;

using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Services;
using EasyKeys.Shipping.Stamps.Rates.Models;
using EasyKeys.Shipping.Stamps.Shipment;
using EasyKeys.Shipping.Stamps.Shipment.DependencyInjection;
using EasyKeys.Shipping.Stamps.Shipment.Models;

using EasyKeysShipping.UnitTest.TestHelpers;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
        var rate = new Rate("USPM", string.Empty, string.Empty, 0m, DateTime.Now) { Name = "USPM" };

        var labels = await _shipmentProvider.CreateShipmentAsync(
              TestShipments.CreateDomesticShipment(),
              new ShipmentDetails(),
              CancellationToken.None);

        Assert.NotNull(labels);
        Assert.NotNull(labels.Labels[0].Bytes[0]);
    }

    [Fact]
    public async Task Process_International_Shipment_Successfully()
    {
        var rate = new Rate("USPMI", string.Empty, string.Empty, 10m, DateTime.Now);

        var shipmentDetails = new ShipmentDetails() { DeclaredValue = 100m, CustomsInformation = new CustomsInformation() { CustomsSigner = "brandon moffett" } };

        var labels = await _shipmentProvider.CreateShipmentAsync(
              TestShipments.CreateInternationalShipment(),
              shipmentDetails,
              CancellationToken.None);

        Assert.NotNull(labels);
        Assert.NotNull(labels.Labels[0].Bytes[0]);
    }

    [Theory]
    [ClassData(typeof(ContentTypeData))]
    public async Task Return_RatesV40_ContentType_Successfully(ContentType contentType, StampsClient.v111.ContentTypeV2 returnedContentType)
    {
        var rateRequest = new RateOptions();

        var shipmentDetails = new ShipmentDetails() { ContentType = contentType };

        var domesticShipment = TestShipments.CreateDomesticShipment();

        var createdShipment = await _shipmentProvider.CreateShipmentAsync(domesticShipment, shipmentDetails, CancellationToken.None);

        Assert.NotNull(createdShipment);
    }

    [Theory]
    [InlineData("Conversation out-of-sync.")]
    public async Task ShipmentProvider_Handles_Exceptions_Successfully(string exMessage)
    {
        // arrange
        var domesticShipment = TestShipments.CreateDomesticShipment();

        var shipmentDetails = new ShipmentDetails();

        var stampsClientMock = new Mock<IStampsClientService>();

        var swsimV111Mock = new Mock<SwsimV111Soap>();

        var rateServiceMock = new Mock<IRatesService>();

        var mockLogger2 = new Mock<ILogger<StampsShipmentProvider>>();

        swsimV111Mock.Setup(x => x.CreateIndiciumAsync(It.IsAny<CreateIndiciumRequest>()))
            .Verifiable();

        swsimV111Mock.SetupSequence(x => x.CreateIndiciumAsync(It.IsAny<CreateIndiciumRequest>()))
            .ThrowsAsync(new FaultException(exMessage))
            .ThrowsAsync(new Exception(exMessage));

        rateServiceMock.Setup(x => x.GetRatesResponseAsync(It.IsAny<Shipment>(), It.IsAny<RateOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<RateV40>());

        var stampsShipmentProvider = new StampsShipmentProvider(stampsClientMock.Object, rateServiceMock.Object, mockLogger2.Object);

        // act
        var result = await stampsShipmentProvider.CreateShipmentAsync(domesticShipment, shipmentDetails, CancellationToken.None);

        swsimV111Mock.Verify(x => x.CreateIndiciumAsync(It.IsAny<CreateIndiciumRequest>()), Times.Exactly(2));

        // assert
        Assert.IsType<ShipmentLabel>(result);

        Assert.Contains(result.InternalErrors, x => x == exMessage);
    }

    [Theory]
    [InlineData("Conversation out-of-sync.")]
    public async Task ShipmentProvider_Refreshes_Token_And_Returns_Shipment_Successfully(string exMessage)
    {
        // arrange
        var domesticShipment = TestShipments.CreateDomesticShipment();

        var shipmentDetails = new ShipmentDetails();

        var stampsClientMock = new Mock<IStampsClientService>();

        var swsimV111Mock = new Mock<SwsimV111Soap>();

        var rateServiceMock = new Mock<IRatesService>();

        var mockLogger2 = new Mock<ILogger<StampsShipmentProvider>>();

        swsimV111Mock.Setup(x => x.CreateIndiciumAsync(It.IsAny<CreateIndiciumRequest>()))
            .Verifiable();

        swsimV111Mock.SetupSequence(x => x.CreateIndiciumAsync(It.IsAny<CreateIndiciumRequest>()))
            .ThrowsAsync(new FaultException(exMessage))
            .ReturnsAsync(new CreateIndiciumResponse()
            { Rate = new RateV40(), ImageData = new byte[0][], Authenticator = "test", StampsTxID = Guid.NewGuid(), TrackingNumber = "test" });

        rateServiceMock.Setup(x => x.GetRatesResponseAsync(It.IsAny<Shipment>(), It.IsAny<RateOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<RateV40>());

        var stampsShipmentProvider = new StampsShipmentProvider(stampsClientMock.Object, rateServiceMock.Object, mockLogger2.Object);

        // act
        var result = await stampsShipmentProvider.CreateShipmentAsync(domesticShipment, shipmentDetails, CancellationToken.None);

        swsimV111Mock.Verify(x => x.CreateIndiciumAsync(It.IsAny<CreateIndiciumRequest>()), Times.Exactly(2));

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
            yield return new object[] { ContentType.CommcercialSample, StampsClient.v111.ContentTypeV2.CommercialSample };

            /* Mailpiece is over maximum weight of 0 lb 15.999 oz
             * Cannot ship First-Class packages larger than 22" x 18" x 15"
             */
            yield return new object[] { ContentType.DangerousGoods, StampsClient.v111.ContentTypeV2.DangerousGoods };

            yield return new object[] { ContentType.Document, StampsClient.v111.ContentTypeV2.Document };

            yield return new object[] { ContentType.Gift, StampsClient.v111.ContentTypeV2.Gift };

            yield return new object[] { ContentType.HumanitarianDonation, StampsClient.v111.ContentTypeV2.HumanitarianDonation };

            // Mail class 'ExpressMailInternational' is not available for the destination country.
            yield return new object[] { ContentType.Merchandise, StampsClient.v111.ContentTypeV2.Merchandise };

            // only for international
            yield return new object[] { ContentType.ReturnedGoods, StampsClient.v111.ContentTypeV2.ReturnedGoods };

            // Mail class UspsReturn not supported.
            // yield return new object[] { ServiceType.USPS_PAY_ON_USE_RETURN, StampsClient.v111.ServiceType.USRETURN };

            yield return new object[] { ContentType.Other, StampsClient.v111.ContentTypeV2.Other };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
