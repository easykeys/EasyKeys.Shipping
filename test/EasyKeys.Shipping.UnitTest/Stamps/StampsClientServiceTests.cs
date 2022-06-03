using System.ServiceModel;

using Bet.Extensions.Testing.Logging;

using EasyKeys.Shipping.Stamps.Abstractions.Options;
using EasyKeys.Shipping.Stamps.Abstractions.Services;

using EasyKeysShipping.UnitTest.Stubs;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

using Moq;

using StampsClient.v111;

namespace EasyKeysShipping.UnitTest.Stamps;

public class StampsClientServiceTests
{
    private readonly ITestOutputHelper _output;
    private readonly IStampsClientService _stampsClient;

    public StampsClientServiceTests(ITestOutputHelper output)
    {
        _output = output;
        _stampsClient = GetStampsClient();
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

        mockSoapClient.Verify(x => x.CleanseAddressAsync(It.IsAny<CleanseAddressRequest>()), Times.Exactly(2));
    }

    private IStampsClientService GetStampsClient()
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

        var sp = services.BuildServiceProvider();
        return sp.GetRequiredService<IStampsClientService>();
    }
}
