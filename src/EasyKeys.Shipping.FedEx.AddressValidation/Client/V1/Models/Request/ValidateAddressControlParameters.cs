using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.AddressValidation.Client.V1.Models.Request;

public class ValidateAddressControlParameters
{
    [JsonPropertyName("includeResolutionTokens")]
    public bool? IncludeResolutionTokens { get; set; }
}
