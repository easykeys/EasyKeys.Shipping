using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Abstractions.Api.V1.Auth.Models;

public record TokenResponse
{
    [JsonPropertyName("token_type")]
    public string TokenType { get; init; } = string.Empty;

    [JsonPropertyName("scope")]
    public string Scope { get; init; } = string.Empty;

    [JsonPropertyName("access_token")]
    public string AccessToken { get; init; } = string.Empty;

    [JsonPropertyName("expires_in")]
    public long ExpiresIn { get; init; }

    public string Token => $"{TokenType} {AccessToken}";
}
