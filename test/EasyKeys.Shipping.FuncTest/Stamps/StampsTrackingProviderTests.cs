using EasyKeys.Shipping.Stamps.Tracking;

using EasyKeysShipping.FuncTest.TestHelpers;

using Microsoft.Extensions.DependencyInjection;

namespace EasyKeysShipping.FuncTest.Stamps;

public class StampsTrackingProviderTests
{
    private readonly IStampsTrackingProvider _trackingProvider;

    public StampsTrackingProviderTests(ITestOutputHelper output)
    {
        _trackingProvider = ServiceProviderInstance.GetStampsServices(output)
            .GetRequiredService<IStampsTrackingProvider>();
    }

    [Fact]
    public async Task Track_Shipment_Successfully()
    {
        var result = await _trackingProvider.TrackShipmentAsync("9400111298370020264221", CancellationToken.None);

        Assert.NotNull(result);
    }
}
