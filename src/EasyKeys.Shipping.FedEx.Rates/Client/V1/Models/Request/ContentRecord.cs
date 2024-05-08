using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;

public class ContentRecord
{
    [JsonPropertyName("itemNumber")]
    public string? ItemNumber { get; set; }

    [JsonPropertyName("receivedQuantity")]
    public int? ReceivedQuantity { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("partNumber")]
    public string? PartNumber { get; set; }
}
