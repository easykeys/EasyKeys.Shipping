using System.Collections.Concurrent;
using System.Net.Http.Json;

using EasyKeys.Shipping.Amazon.Abstractions.OpenApis.V2.Shipping;
using EasyKeys.Shipping.Amazon.Abstractions.Options;

using Microsoft.Extensions.Logging;

using Microsoft.Extensions.Options;

namespace EasyKeys.Shipping.Amazon.Abstractions.Services.Impl;

public class AmazonApiAuthenticatorService : IAmazonApiAuthenticatorService
{
    private readonly AmazonShippingApi _httpClient;
    private readonly AmazonShippingApiOptions _options;
    private readonly ILogger<AmazonApiAuthenticatorService> _logger;
    private readonly ConcurrentDictionary<string, string> _token = new();
    private readonly ConcurrentDictionary<string, DateTimeOffset> _expirationClock = new();

    public AmazonApiAuthenticatorService(
        AmazonShippingApi httpClient,
        IOptionsMonitor<AmazonShippingApiOptions> optionsMonitor,
        ILogger<AmazonApiAuthenticatorService> logger)
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
                _token.TryGetValue(nameof(AmazonToken), out var existingToken);
                ArgumentNullException.ThrowIfNull(existingToken, nameof(AmazonToken));
                return existingToken;
            }

            using var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://api.amazon.com/auth/o2/token"),
                Headers =
                {
                    { "accept", "application/json" },
                },
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "grant_type", "refresh_token" },
                    { "refresh_token", _options.RefreshToken },
                    { "client_id", _options.ClientId },
                    { "client_secret", _options.ClientSecret }
                }),
            };
            var response = await client.SendAsync(request);
            var token = await response.Content.ReadFromJsonAsync<AmazonToken>();

            _logger.LogDebug("[Amazon][CreateToken] - authentication token returned {createdToken}", token);

            ArgumentNullException.ThrowIfNull(token, nameof(AmazonToken));

            _token.AddOrUpdate(nameof(AmazonToken), $"{token.access_token}", (x, y) => $"{token.access_token}");

            _expirationClock.AddOrUpdate(nameof(_expirationClock), (x) => DateTimeOffset.Now.AddSeconds(token.expires_in - 5), (x, y) => y.AddMilliseconds(token.expires_in - 5));

            return $"{token.token_type} {token.access_token}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Fedex.com][GetToken] Failed");
            return string.Empty;
        }
    }
}
