using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;

public class Quantity
{
    [JsonPropertyName("quantityType")]
    public string? QuantityType { get; set; }

    [JsonPropertyName("amount")]
    public int? Amount { get; set; }

    [JsonPropertyName("units")]
    public string? Units { get; set; }
}
