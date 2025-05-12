using System.Collections.Concurrent;

using EasyKeys.Shipping.Amazon.Abstractions.OpenApis.V2.Shipping;
using EasyKeys.Shipping.Amazon.Abstractions.Options;

using Microsoft.Extensions.Logging;

using Microsoft.Extensions.Options;

namespace EasyKeys.Shipping.Amazon.Abstractions.Services.Impl;

public class AmazonApiAuthenticatorService : IAmazonApiAuthenticatorService
{
    private readonly AmazonShippingApiOptions _options;
    private readonly ILogger<AmazonApiAuthenticatorService> _logger;
    private readonly ConcurrentDictionary<string, string> _token = new();
    private readonly ConcurrentDictionary<string, DateTimeOffset> _expirationClock = new();

    public AmazonApiAuthenticatorService(
        IOptionsMonitor<AmazonShippingApiOptions> optionsMonitor,
        ILogger<AmazonApiAuthenticatorService> logger)
    {
        _options = optionsMonitor.CurrentValue;
        _logger = logger;
    }

    public async Task<string> GetTokenAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (DateTimeOffset.Now < _expirationClock.GetValueOrDefault(nameof(_expirationClock)))
            {
                _token.TryGetValue(nameof(AmazonToken), out var existingToken);

                if (existingToken == null)
                {
                    throw new ArgumentNullException(nameof(existingToken), "Token is null or expired.");
                }

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
            var token = await response.Content.ReadAsStringAsync();
            var tokenResult = System.Text.Json.JsonSerializer.Deserialize<AmazonToken>(token);

            _logger.LogDebug("[Amazon][CreateToken] - authentication token returned {createdToken}", token);

            if (tokenResult == null)
            {
                throw new ArgumentNullException(nameof(tokenResult), "Token result is null.");
            }

            _token.AddOrUpdate(nameof(AmazonToken), $"{tokenResult.access_token}", (x, y) => $"{tokenResult.access_token}");

            _expirationClock.AddOrUpdate(nameof(_expirationClock), (x) => DateTimeOffset.Now.AddSeconds(tokenResult.expires_in - 5), (x, y) => y.AddMilliseconds(tokenResult.expires_in - 5));

            return tokenResult.access_token;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Fedex.com][GetToken] Failed");
            return string.Empty;
        }
    }
}
