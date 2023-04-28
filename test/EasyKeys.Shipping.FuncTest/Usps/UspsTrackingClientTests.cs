using EasyKeys.Shipping.Usps.Tracking;

using EasyKeysShipping.FuncTest.TestHelpers;

using Microsoft.Extensions.DependencyInjection;

namespace EasyKeysShipping.FuncTest.Usps;

public class UspsTrackingClientTests
{
    private readonly IServiceProvider _sp;

    public UspsTrackingClientTests(ITestOutputHelper output)
    {
        _sp = ServiceProviderInstance.GetUspsServices(output);
    }

    /// <summary>
    /// Provide current usps shipment to work.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task TestUspsTrackingClientAsync()
    {
        var client = _sp.GetRequiredService<IUspsTrackingClient>();

        var result = await client.GetTrackInfoAsync("9400111298370588642059", CancellationToken.None);

        Assert.NotNull(result);
    }
}
