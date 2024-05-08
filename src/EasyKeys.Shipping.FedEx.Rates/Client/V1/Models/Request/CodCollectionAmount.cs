using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;

public class CodCollectionAmount
{
    [JsonPropertyName("amount")]
    public double? Amount { get; set; }

    [JsonPropertyName("currency")]
    public string? Currency { get; set; }
}
