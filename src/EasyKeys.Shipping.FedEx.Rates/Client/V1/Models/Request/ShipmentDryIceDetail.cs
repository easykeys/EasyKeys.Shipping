using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;

public class ShipmentDryIceDetail
{
    [JsonPropertyName("totalWeight")]
    public TotalWeight? TotalWeight { get; set; }

    [JsonPropertyName("packageCount")]
    public int? PackageCount { get; set; }
}
