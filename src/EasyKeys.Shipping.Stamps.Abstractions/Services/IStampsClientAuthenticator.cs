namespace EasyKeys.Shipping.Stamps.Abstractions.Services;

public interface IStampsClientAuthenticator
{
    Task<string> CreateTokenAsync();

    void SetToken(string token);

    string GetToken();

    void ClearTokens();
}
