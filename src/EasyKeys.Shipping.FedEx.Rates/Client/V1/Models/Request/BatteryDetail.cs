using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;

public class BatteryDetail
{
    [JsonPropertyName("material")]
    public string? Material { get; set; }

    [JsonPropertyName("regulatorySubType")]
    public string? RegulatorySubType { get; set; }

    [JsonPropertyName("packing")]
    public string? Packing { get; set; }
}
