using StampsClient.v111;

namespace EasyKeys.Shipping.Stamps.Abstractions.Services;

public interface IStampsClientService
{
    /// <summary>
    /// Creates Stamps.com SOAP client.
    /// </summary>
    /// <returns></returns>
    SwsimV111Soap CreateClient();

    /// <summary>
    /// Gets authentication token to be used for subsequent calls.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<string> GetTokenAsync(CancellationToken cancellationToken);
}
