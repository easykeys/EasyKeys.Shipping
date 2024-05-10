namespace EasyKeys.Shipping.FedEx.Abstractions.Services;
public interface IFedexApiAuthenticatorService
{
    Task<string> GetTokenAsync(CancellationToken cancellationToken = default);
}
