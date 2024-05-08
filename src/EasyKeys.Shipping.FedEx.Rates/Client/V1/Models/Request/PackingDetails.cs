using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;

public class PackingDetails
{
    [JsonPropertyName("packingInstructions")]
    public string? PackingInstructions { get; set; }

    [JsonPropertyName("cargoAircraftOnly")]
    public bool? CargoAircraftOnly { get; set; }
}
