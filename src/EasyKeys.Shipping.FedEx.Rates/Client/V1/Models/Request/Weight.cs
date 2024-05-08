using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;

public class Weight
{
    [JsonPropertyName("units")]
    public string? Units { get; set; }

    [JsonPropertyName("value")]
    public int? Value { get; set; }
}
