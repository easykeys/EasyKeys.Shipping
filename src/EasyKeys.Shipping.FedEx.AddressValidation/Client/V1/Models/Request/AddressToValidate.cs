using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.AddressValidation.Client.V1.Models.Request;

public class AddressesToValidate
{
    [JsonPropertyName("address")]
    required public Address Address { get; set; }

    [JsonPropertyName("clientReferenceId")]
    public string? ClientReferenceId { get; set; }
}
