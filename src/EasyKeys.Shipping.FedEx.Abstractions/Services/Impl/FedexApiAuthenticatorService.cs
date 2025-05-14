using System.Collections.Concurrent;
using EasyKeys.Shipping.FedEx.Abstractions.OpenApis.V1.Authorization;
using EasyKeys.Shipping.FedEx.Abstractions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EasyKeys.Shipping.FedEx.Abstractions.Services.Impl;
public class FedexApiAuthenticatorService : IFedexApiAuthenticatorService
{
    private readonly AuthorizationApi _httpClient;
    private readonly FedExApiOptions _options;
    private readonly ILogger<FedexApiAuthenticatorService> _logger;
    private readonly ConcurrentDictionary<string, string> _token = new();
    private readonly ConcurrentDictionary<string, DateTimeOffset> _expirationClock = new();

    public FedexApiAuthenticatorService(
        AuthorizationApi httpClient,
        IOptionsMonitor<FedExApiOptions> optionsMonitor,
        ILogger<FedexApiAuthenticatorService> logger)
    {
        _options = optionsMonitor.CurrentValue;
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<string> GetTokenAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (DateTimeOffset.Now < _expirationClock.GetValueOrDefault(nameof(_expirationClock)))
            {
                _token.TryGetValue(nameof(Response), out var existingToken);

                if(existingToken == null)
                {
                    throw new ArgumentNullException(nameof(Response), "Token is null or expired."); 
                }

                return existingToken;
            }

            var token = await _httpClient.API_AuthorizationAsync(
                "application/json",
                new() { Client_id = _options.ClientId, Client_secret = _options.ClientSecret, Grant_type = "client_credentials" },
                cancellationToken);

            _logger.LogDebug("[Fedex.com][CreateToken] - authentication token returned {createdToken}", token);

            if (token == null)
            {
                throw new ArgumentNullException("Token", "Token is null.");
            }

            _token.AddOrUpdate(nameof(Response), $"{token.Token_type} {token.Access_token}", (x, y) => $"{token.Token_type} {token.Access_token}");

            _expirationClock.AddOrUpdate(nameof(_expirationClock), (x) => DateTimeOffset.Now.AddSeconds(token.Expires_in - 5), (x, y) => y.AddMilliseconds(token.Expires_in - 5));

            return $"{token.Token_type} {token.Access_token}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Fedex.com][GetToken] Failed");
            return string.Empty;
        }
    }
}
