using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.AddressValidation.Client.V1.Models.Request;
public class RequestRoot
{
    [JsonPropertyName("inEffectAsOfTimestamp")]
    public string? InEffectAsOfTimestamp { get; set; }

    [JsonPropertyName("validateAddressControlParameters")]
    public ValidateAddressControlParameters? ValidateAddressControlParameters { get; set; }

    [JsonPropertyName("addressesToValidate")]
    required public List<AddressesToValidate> AddressesToValidate { get; set; }
}
