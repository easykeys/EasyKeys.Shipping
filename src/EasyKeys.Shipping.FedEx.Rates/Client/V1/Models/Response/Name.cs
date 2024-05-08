using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Response;

public class Name
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("encoding")]
    public string? Encoding { get; set; }

    [JsonPropertyName("value")]
    public string? Value { get; set; }
}
