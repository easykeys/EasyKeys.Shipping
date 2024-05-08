using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;

public class Container
{
    [JsonPropertyName("offeror")]
    public string? Offeror { get; set; }

    [JsonPropertyName("hazardousCommodities")]
    public List<HazardousCommodity>? HazardousCommodities { get; set; }

    [JsonPropertyName("numberOfContainers")]
    public int? NumberOfContainers { get; set; }

    [JsonPropertyName("containerType")]
    public string? ContainerType { get; set; }

    [JsonPropertyName("emergencyContactNumber")]
    public EmergencyContactNumber? EmergencyContactNumber { get; set; }

    [JsonPropertyName("packaging")]
    public Packaging? Packaging { get; set; }

    [JsonPropertyName("packingType")]
    public string? PackingType { get; set; }

    [JsonPropertyName("radioactiveContainerClass")]
    public string? RadioactiveContainerClass { get; set; }
}
