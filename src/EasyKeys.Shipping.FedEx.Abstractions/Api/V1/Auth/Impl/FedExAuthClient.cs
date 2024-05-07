using System.Collections.Concurrent;
using System.Net.Http.Json;

using EasyKeys.Shipping.FedEx.Abstractions.Api.V1.Auth;
using EasyKeys.Shipping.FedEx.Abstractions.Api.V1.Auth.Models;
using EasyKeys.Shipping.FedEx.Abstractions.Options;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EasyKeys.Shipping.FedEx.Abstractions.Api.V1.Auth.Impl;

public class FedExAuthClient : IFedExAuthClient
{
    private readonly HttpClient _httpClient;
    private readonly FedExApiOptions _options;
    private readonly ILogger<FedExAuthClient> _logger;
    private readonly ConcurrentDictionary<string, string> _token = new();
    private readonly ConcurrentDictionary<string, DateTimeOffset> _expirationClock = new();

    public FedExAuthClient(
        IHttpClientFactory clientFactory,
        IOptionsMonitor<FedExApiOptions> optionsMonitor,
        ILogger<FedExAuthClient> logger)
    {
        _options = optionsMonitor.CurrentValue;
        _httpClient = clientFactory.CreateClient(nameof(IFedExAuthClient));
        _logger = logger;
    }

    public async Task<string> GetTokenAsync()
    {
        try
        {
            if (DateTimeOffset.Now < _expirationClock.GetValueOrDefault(nameof(_expirationClock)))
            {
                _logger.LogDebug("Token is returned");
                _token.TryGetValue(nameof(TokenResponse), out var existingToken);
                ArgumentNullException.ThrowIfNull(existingToken, nameof(TokenResponse));
                return existingToken;
            }

            _logger.LogDebug("Token being requested.");

            // Send a POST request to FedEx's token endpoint
            var response = await _httpClient.PostAsync($"{_options.Url}oauth/token", new FormUrlEncodedContent(new Dictionary<string, string>
                                {
                                    { "client_id", _options.ClientId },
                                    { "client_secret", _options.ClientSecret },
                                    { "grant_type", "client_credentials" }
                                }));

            response.EnsureSuccessStatusCode();

            var tokenObj = await response.Content.ReadFromJsonAsync<TokenResponse>();

            ArgumentNullException.ThrowIfNull(tokenObj, nameof(TokenResponse));

            _logger.LogInformation("Token being updated");

            _token.AddOrUpdate(nameof(TokenResponse), tokenObj.Token, (x, y) => tokenObj.Token);

            _expirationClock.AddOrUpdate(nameof(_expirationClock), (x) => DateTimeOffset.Now.AddSeconds(tokenObj.ExpiresIn - 5), (x, y) => y.AddSeconds(tokenObj.ExpiresIn - 5));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }

        _token.TryGetValue(nameof(TokenResponse), out var token);
        return token;
    }
}
