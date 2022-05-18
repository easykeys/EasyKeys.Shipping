using System.Collections.Concurrent;
using System.ServiceModel;

using EasyKeys.Shipping.Stamps.Abstractions.Options;

using Microsoft.Extensions.Options;

using StampsClient.v111;

namespace EasyKeys.Shipping.Stamps.Abstractions.Services.Impl;

internal sealed class StampsClientService : IStampsClientService
{
    private StampsOptions _options;
    private ConcurrentDictionary<string, string> _token = new ConcurrentDictionary<string, string>();

    public StampsClientService(IOptionsMonitor<StampsOptions> optionsMonitor)
    {
        _options = optionsMonitor.CurrentValue;

        optionsMonitor.OnChange(x => _options = x);
    }

    public SwsimV111Soap CreateClient()
    {
        return new SwsimV111SoapClient(
             new BasicHttpsBinding(BasicHttpsSecurityMode.Transport)
             {
                 MaxReceivedMessageSize = int.MaxValue,
             },
             new EndpointAddress(_options.Url));
    }

    public async Task<string> GetTokenAsync(CancellationToken cancellationToken)
    {
        if (_token.TryGetValue("token", out var savedToken))
        {
            return savedToken;
        }

        return await AuthenticateUser(cancellationToken);
    }

    public bool SetToken(string newToken)
    {
        if (_token.TryGetValue("token", out var oldToken))
        {
            return _token.TryUpdate("token", newToken, oldToken);
        }

        return _token.TryAdd("token", newToken);
    }

    public async Task<string> RefreshTokenAsync(CancellationToken cancellationToken)
    {
        return await AuthenticateUser(cancellationToken);
    }

    private async Task<string> AuthenticateUser(CancellationToken cancellationToken)
    {
        var credentials = new Credentials()
        {
            IntegrationID = new Guid(_options.IntegrationId),
            Username = _options.UserName,
            Password = _options.Password
        };

        var authRequest = new AuthenticateUserRequest(credentials);

        var client = CreateClient();

        var authResponse = await client.AuthenticateUserAsync(authRequest);

        SetToken(authResponse.Authenticator);

        return authResponse.Authenticator;
    }
}
