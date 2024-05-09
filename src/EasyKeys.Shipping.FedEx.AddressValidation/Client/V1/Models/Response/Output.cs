using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.AddressValidation.Client.V1.Models.Response;

public class Output
{
    [JsonPropertyName("resolvedAddresses")]
    public List<ResolvedAddress>? ResolvedAddresses { get; set; }

    [JsonPropertyName("alerts")]
    public List<Alert> Alerts { get; set; } = new ();
}
