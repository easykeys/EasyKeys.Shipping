using EasyKeys.Shipping.Stamps.Abstractions.Options;
using EasyKeys.Shipping.Stamps.Abstractions.Services;
using EasyKeys.Shipping.Stamps.Rates;
using EasyKeys.Shipping.Stamps.Rates.Models;

using EasyKeysShipping.UnitTest.Stubs;
using EasyKeysShipping.UnitTest.TestHelpers;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

using Moq;

using StampsClient.v111;

namespace EasyKeysShipping.UnitTest.Stamps;

public class StampsRateProviderTests
{
    private readonly ITestOutputHelper _output;

    public StampsRateProviderTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task Return_Shipment_With_Rates_Successfully()
    {
        var rateV40List = new List<RateV40>()
        {
            new RateV40()
            {
                ServiceType = ServiceType.USFC,
                ServiceDescription = "TestDescription",
                Amount = 100m,
                DeliveryDate = DateTime.Now
            }
        };

        var mockOptions = new Mock<IOptionsMonitor<StampsOptions>>();
        var mockAuth = new Mock<IStampsClientAuthenticator>();
        var mockSoapClient = new Mock<SwsimV111Soap>();
        var loggerFactory = new NullLoggerFactory();
        var mockLogger = new Mock<ILogger<StampsClientServiceMock>>();

        var stampsClient = new StampsClientServiceMock(
                                mockOptions.Object,
                                mockAuth.Object,
                                mockSoapClient.Object,
                                loggerFactory,
                                mockLogger.Object);

        var iLoggerMock = new Mock<ILogger<StampsRateProvider>>();

        var stampsRateProvider = new StampsRateProvider(
                                     stampsClient,
                                     iLoggerMock.Object);

        var returnedShipment = await stampsRateProvider.GetDomesticRatesAsync(
            TestShipments.CreateDomesticShipment(),
            new RateOptions(),
            CancellationToken.None);

        Assert.NotNull(returnedShipment);

        Assert.True(returnedShipment.Rates.All(x => x.Name == ServiceType.USFC.ToString()));

        Assert.True(returnedShipment.Rates.All(x => x.ServiceName == "TestDescription"));

        Assert.True(returnedShipment.Rates.All(x => x.TotalCharges == 100m));

        Assert.True(returnedShipment.Rates.All(x => x.GuaranteedDelivery.GetValueOrDefault().Day == DateTime.Now.Day));
    }

    [Fact]
    public async Task Return_Shipment_With_InternalErrors()
    {
        var rateV40List = new List<RateV40>()
        {
            new RateV40()
            {
                ServiceType = ServiceType.USFC,
                ServiceDescription = "TestDescription",
                Amount = 100m,
                DeliveryDate = DateTime.Now
            }
        };

        var mockOptions = new Mock<IOptionsMonitor<StampsOptions>>();
        var mockAuth = new Mock<IStampsClientAuthenticator>();
        var mockSoapClient = new Mock<SwsimV111Soap>();
        var loggerFactory = new NullLoggerFactory();
        var mockLogger = new Mock<ILogger<StampsClientServiceMock>>();

        var stampsClient = new StampsClientServiceMock(
                                mockOptions.Object,
                                mockAuth.Object,
                                mockSoapClient.Object,
                                loggerFactory,
                                mockLogger.Object);

        var iLoggerMock = new Mock<ILogger<StampsRateProvider>>();

        var stampsRateProvider = new StampsRateProvider(
                                     stampsClient,
                                     iLoggerMock.Object);

        var returnedShipment = await stampsRateProvider.GetDomesticRatesAsync(
            TestShipments.CreateDomesticShipment(),
            new RateOptions(),
            CancellationToken.None);

        Assert.NotNull(returnedShipment);

        Assert.Equal("Test SOAP Exception", returnedShipment.InternalErrors.FirstOrDefault());
    }
}
