namespace EasyKeys.Shipping.FedEx.Abstractions.Api.V1.Auth;

/// <summary>
/// Implmentation of the FedEx Rates and Transit Times client. Can be used to get rates and transit times.
/// <see href="https://developer.fedex.com/api/en-us/catalog/authorization/v1/docs.html"/>.
/// </summary>
public interface IFedExAuthClient
{
    /// <summary>
    /// Gets the authentication token asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The authentication token.</returns>
    Task<string> GetTokenAsync(CancellationToken cancellationToken = default);
}
