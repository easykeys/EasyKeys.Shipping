using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Abstractions.Models;
public class ApiError
{
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}
