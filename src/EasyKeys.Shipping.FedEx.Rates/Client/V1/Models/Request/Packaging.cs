using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;

public class Packaging
{
    [JsonPropertyName("count")]
    public int? Count { get; set; }

    [JsonPropertyName("units")]
    public string? Units { get; set; }
}
