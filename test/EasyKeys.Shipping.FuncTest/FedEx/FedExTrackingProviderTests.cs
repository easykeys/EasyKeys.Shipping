using EasyKeys.Shipping.FedEx.Tracking;

using EasyKeysShipping.FuncTest.TestHelpers;

using Microsoft.Extensions.DependencyInjection;

namespace EasyKeysShipping.FuncTest.FedEx;

public class FedExTrackingProviderTests
{
    private readonly ITestOutputHelper _output;
    private readonly IFedExTrackingProvider _trackingProvider;

    public FedExTrackingProviderTests(ITestOutputHelper output)
    {
        _output = output;
        _trackingProvider = ServiceProviderInstance.GetFedExServices(output)
            .GetRequiredService<IFedExTrackingProvider>();
    }

    [Theory]
    [InlineData("778161615475509")]
    [InlineData("776753740455")]
    [InlineData("272707764259")]
    [InlineData("272712649472")]
    [InlineData("272719656012")]
    public async Task Track_Shipment_Successfully(string trackingId)
    {
        var result = await _trackingProvider.TrackShipmentAsync(trackingId, CancellationToken.None);

        Assert.NotNull(result.TrackingEvents);
    }
}
