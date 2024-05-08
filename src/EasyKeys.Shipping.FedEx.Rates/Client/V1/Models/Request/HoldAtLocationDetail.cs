using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;

public class HoldAtLocationDetail
{
    [JsonPropertyName("locationId")]
    public string? LocationId { get; set; }

    [JsonPropertyName("locationContactAndAddress")]
    public LocationContactAndAddress? LocationContactAndAddress { get; set; }

    [JsonPropertyName("locationType")]
    public string? LocationType { get; set; }
}
