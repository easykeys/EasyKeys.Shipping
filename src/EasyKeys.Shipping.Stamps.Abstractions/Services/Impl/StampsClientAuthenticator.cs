using System.Collections.Concurrent;
using System.ServiceModel;

using EasyKeys.Shipping.Stamps.Abstractions.Options;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using StampsClient.v111;

namespace EasyKeys.Shipping.Stamps.Abstractions.Services.Impl;

internal class StampsClientAuthenticator : IStampsClientAuthenticator
{
    private StampsOptions _options;
    private readonly ILogger<StampsClientAuthenticator> _logger;

    private readonly ConcurrentBag<string> _tokens = new();

    public StampsClientAuthenticator(
        IOptionsMonitor<StampsOptions> optionsMonitor,
        ILogger<StampsClientAuthenticator> logger)
    {
        _options = optionsMonitor.CurrentValue;

        optionsMonitor.OnChange(x => _options = x);
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<string> CreateTokenAsync()
    {
        var client = new SwsimV111SoapClient(
         new BasicHttpsBinding(BasicHttpsSecurityMode.Transport)
         {
             MaxReceivedMessageSize = int.MaxValue,
         },
         new EndpointAddress(_options.Url));

        var credentials = new Credentials()
        {
            IntegrationID = new Guid(_options.IntegrationId),
            Username = _options.UserName,
            Password = _options.Password
        };

        var authRequest = new AuthenticateUserRequest(credentials);

        var authResponse = await client.AuthenticateUserAsync(authRequest);

        var token = authResponse.Authenticator;

        _logger.LogDebug("[Stamps.com][CreateToken] - authentication token returned {createdToken}", token);

        return token;
    }

    public void SetToken(string token)
    {
        _tokens.Add(token);
    }

    public string GetToken()
    {
        if (_tokens.TryTake(out var token))
        {
            return token;
        }

        _logger.LogDebug("[Stamps.com][GetToken]- requesting new authetntication token.");

        return CreateTokenAsync().GetAwaiter().GetResult();
    }

    public void ClearTokens()
    {
        while (_tokens.Count > 0)
        {
            _tokens.TryTake(out var result);
        }
    }
}
