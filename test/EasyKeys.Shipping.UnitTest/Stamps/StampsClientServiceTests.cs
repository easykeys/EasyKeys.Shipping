using System.ServiceModel;

using EasyKeys.Shipping.Stamps.Abstractions.Options;
using EasyKeys.Shipping.Stamps.Abstractions.Services;

using EasyKeysShipping.UnitTest.Stubs;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

using Moq;

using StampsClient.v111;

namespace EasyKeysShipping.UnitTest.Stamps;

public class StampsClientServiceTests
{
    private readonly ITestOutputHelper _output;

    public StampsClientServiceTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Theory]
    [InlineData("Conversation out-of-sync.")]
    [InlineData("Invalid conversation token.")]
    [InlineData("Authentication failed.")]
    public async Task CleanseAddress_Successfully(string exMessage)
    {
        // arrange
        var mockOptions = new Mock<IOptionsMonitor<StampsOptions>>();
        var mockAuth = new Mock<IStampsClientAuthenticator>();
        var loggerFactory = new NullLoggerFactory();
        var mockLogger = new Mock<ILogger<StampsClientServiceMock>>();
        var mockSoapClient = new Mock<SwsimV111Soap>();

        mockOptions.Setup(x => x.CurrentValue).Returns(new StampsOptions());
        mockAuth.Setup(x => x.GetToken()).Returns("testing");
        mockAuth.Setup(x => x.ClearTokens()).Verifiable();

        mockSoapClient.Setup(x => x.CleanseAddressAsync(It.IsAny<CleanseAddressRequest>()))
            .Verifiable();

        mockSoapClient.SetupSequence(x => x.CleanseAddressAsync(It.IsAny<CleanseAddressRequest>()))
            .ThrowsAsync(new FaultException(exMessage))
            .ReturnsAsync(new CleanseAddressResponse() { Authenticator = "testing" });

        var stampsClient = new StampsClientServiceMock(
            mockOptions.Object,
            mockAuth.Object,
            mockSoapClient.Object,
            loggerFactory,
            mockLogger.Object);

        // act
        var result = await stampsClient.CleanseAddressAsync(new CleanseAddressRequest(), CancellationToken.None);
        mockAuth.Verify(x => x.ClearTokens(), Times.Once);

        mockSoapClient.Verify(x => x.CleanseAddressAsync(It.IsAny<CleanseAddressRequest>()), Times.Exactly(2));
    }

    [Theory]
    [InlineData("Conversation out-of-sync.")]
    [InlineData("Invalid conversation token.")]
    [InlineData("Authentication failed.")]
    public async Task GetRates_Successfully(string exMessage)
    {
        // arrange
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
            .ReturnsAsync(new GetRatesResponse() { Authenticator = "testing" });

        var stampsClient = new StampsClientServiceMock(
            mockOptions.Object,
            mockAuth.Object,
            mockSoapClient.Object,
            loggerFactory,
            mockLogger.Object);

        // act
        var result = await stampsClient.GetRatesAsync(new GetRatesRequest(), CancellationToken.None);
        mockAuth.Verify(x => x.ClearTokens(), Times.Once);
        mockSoapClient.Verify(x => x.GetRatesAsync(It.IsAny<GetRatesRequest>()), Times.Exactly(2));
    }

    [Theory]
    [InlineData("Conversation out-of-sync.")]
    [InlineData("Invalid conversation token.")]
    [InlineData("Authentication failed.")]
    public async Task CreateIndicium_Successfully(string exMessage)
    {
        // arrange
        var mockOptions = new Mock<IOptionsMonitor<StampsOptions>>();
        var mockAuth = new Mock<IStampsClientAuthenticator>();
        var loggerFactory = new NullLoggerFactory();
        var mockLogger = new Mock<ILogger<StampsClientServiceMock>>();
        var mockSoapClient = new Mock<SwsimV111Soap>();

        mockOptions.Setup(x => x.CurrentValue).Returns(new StampsOptions());
        mockAuth.Setup(x => x.GetToken()).Returns("testing");
        mockAuth.Setup(x => x.ClearTokens()).Verifiable();
        mockSoapClient.Setup(x => x.CreateIndiciumAsync(It.IsAny<CreateIndiciumRequest>()))
            .Verifiable();

        mockSoapClient.SetupSequence(x => x.CreateIndiciumAsync(It.IsAny<CreateIndiciumRequest>()))
            .ThrowsAsync(new FaultException(exMessage))
            .ReturnsAsync(new CreateIndiciumResponse() { Authenticator = "testing" });

        var stampsClient = new StampsClientServiceMock(
            mockOptions.Object,
            mockAuth.Object,
            mockSoapClient.Object,
            loggerFactory,
            mockLogger.Object);

        // act
        var result = await stampsClient.CreateIndiciumAsync(new CreateIndiciumRequest(), CancellationToken.None);
        mockAuth.Verify(x => x.ClearTokens(), Times.Once);
        mockSoapClient.Verify(x => x.CreateIndiciumAsync(It.IsAny<CreateIndiciumRequest>()), Times.Exactly(2));
    }
}
