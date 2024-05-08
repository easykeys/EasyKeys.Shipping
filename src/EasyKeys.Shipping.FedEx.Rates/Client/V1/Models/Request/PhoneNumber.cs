using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;

public class PhoneNumber
{
    [JsonPropertyName("areaCode")]
    public string? AreaCode { get; set; }

    [JsonPropertyName("extension")]
    public string? Extension { get; set; }

    [JsonPropertyName("countryCode")]
    public string? CountryCode { get; set; }

    [JsonPropertyName("personalIdentificationNumber")]
    public string? PersonalIdentificationNumber { get; set; }

    [JsonPropertyName("localNumber")]
    public string? LocalNumber { get; set; }
}
