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

    /// <summary>
    /// Use thread safe concurrent property to save token for reuse.
    /// </summary>
    /// <param name="token"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    bool SetToken(string token);

    /// <summary>
    /// <para>
    /// If your application is idle for an extended period of time then the Authenticator token
    /// will expire.At this point, you will get a ”conversation‐out‐of‐sync” error when you make an SWS/IM call
    /// informing you that the token has expired.You should then log the user back in using AuthenticateUser
    /// and use the fresh token to continue.
    /// </para>
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<string> RefreshTokenAsync(CancellationToken cancellationToken);
}
