using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;

public class DangerousGoodsDetail
{
    [JsonPropertyName("offeror")]
    public string? Offeror { get; set; }

    [JsonPropertyName("accessibility")]
    public string? Accessibility { get; set; }

    [JsonPropertyName("emergencyContactNumber")]
    public string? EmergencyContactNumber { get; set; }

    [JsonPropertyName("options")]
    public List<string>? Options { get; set; }

    [JsonPropertyName("containers")]
    public List<Container>? Containers { get; set; }

    [JsonPropertyName("regulation")]
    public string? Regulation { get; set; }

    [JsonPropertyName("packaging")]
    public Packaging? Packaging { get; set; }
}
