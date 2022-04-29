using System.ServiceModel;

using EasyKeys.Shipping.Stamps.Abstractions.Options;

using Microsoft.Extensions.Options;

using StampsClient.v111;

namespace EasyKeys.Shipping.Stamps.Abstractions.Services.Impl;

internal sealed class StampsClientService : IStampsClientService
{
    private StampsOptions _options;

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
        var credentials = new Credentials()
        {
            IntegrationID = new Guid(_options.IntegrationId),
            Username = _options.UserName,
            Password = _options.Password
        };

        var authRequest = new AuthenticateUserRequest(credentials);
        var client = CreateClient();
        var authResponse = await client.AuthenticateUserAsync(authRequest);
        return authResponse.Authenticator;
    }
}
