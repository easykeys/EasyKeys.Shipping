using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.AddressValidation.Client.V1.Models.Response;

public class ParsedPostalCode
{
    [JsonPropertyName("base")]
    public string? Base { get; set; }

    [JsonPropertyName("addOn")]
    public string? AddOn { get; set; }

    [JsonPropertyName("deliveryPoint")]
    public string? DeliveryPoint { get; set; }
}
