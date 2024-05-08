using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.AddressValidation.Client.V1.Models.Request;

public class Address
{
    [JsonPropertyName("streetLines")]
    public string[]? StreetLines { get; set; }

    [JsonPropertyName("city")]
    public string? City { get; set; }

    [JsonPropertyName("stateOrProvinceCode")]
    public string? StateOrProvinceCode { get; set; }

    [JsonPropertyName("postalCode")]
    required public string PostalCode { get; set; }

    [JsonPropertyName("countryCode")]
    required public string CountryCode { get; set; }
}
