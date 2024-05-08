using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;

public class BrokerAddress
{
    [JsonPropertyName("streetLines")]
    public List<string>? StreetLines { get; set; }

    [JsonPropertyName("city")]
    public string? City { get; set; }

    [JsonPropertyName("stateOrProvinceCode")]
    public string? StateOrProvinceCode { get; set; }

    [JsonPropertyName("postalCode")]
    public string? PostalCode { get; set; }

    [JsonPropertyName("countryCode")]
    public string? CountryCode { get; set; }

    [JsonPropertyName("residential")]
    public bool? Residential { get; set; }

    [JsonPropertyName("classification")]
    public string? Classification { get; set; }

    [JsonPropertyName("geographicCoordinates")]
    public string? GeographicCoordinates { get; set; }

    [JsonPropertyName("urbanizationCode")]
    public string? UrbanizationCode { get; set; }

    [JsonPropertyName("countryName")]
    public string? CountryName { get; set; }
}
