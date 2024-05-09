using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.AddressValidation.Client.V1.Models.Response;

public class ResolutionToken
{
    [JsonPropertyName("changed")]
    public bool? Changed { get; set; }

    [JsonPropertyName("value")]
    public string? Value { get; set; }
}
