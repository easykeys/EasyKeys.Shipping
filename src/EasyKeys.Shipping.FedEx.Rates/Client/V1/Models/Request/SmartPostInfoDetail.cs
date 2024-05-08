using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;

public class SmartPostInfoDetail
{
    [JsonPropertyName("ancillaryEndorsement")]
    public string? AncillaryEndorsement { get; set; }

    [JsonPropertyName("hubId")]
    public string? HubId { get; set; }

    [JsonPropertyName("indicia")]
    public string? Indicia { get; set; }

    [JsonPropertyName("specialServices")]
    public string? SpecialServices { get; set; }
}
