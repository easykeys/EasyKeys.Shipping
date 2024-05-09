using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.AddressValidation.Client.V1.Models.Response;

public class ResolvedAddress
{
    [JsonPropertyName("streetLinesToken")]
    public string[]? StreetLinesToken { get; set; }

    [JsonPropertyName("city")]
    public string? City { get; set; }

    [JsonPropertyName("stateOrProvinceCode")]
    public string? StateOrProvinceCode { get; set; }

    [JsonPropertyName("countryCode")]
    public string? CountryCode { get; set; }

    [JsonPropertyName("customerMessage")]
    public List<ResolutionToken>? CustomerMessage { get; set; }

    [JsonPropertyName("cityToken")]
    public List<ResolutionToken>? CityToken { get; set; }

    [JsonPropertyName("postalCodeToken")]
    public ResolutionToken? PostalCodeToken { get; set; }

    [JsonPropertyName("parsedPostalCode")]
    public ParsedPostalCode? ParsedPostalCode { get; set; }

    [JsonPropertyName("classification")]
    public string? Classification { get; set; }

    [JsonPropertyName("postOfficeBox")]
    public bool? PostOfficeBox { get; set; }

    [JsonPropertyName("normalizedStatusNameDPV")]
    public bool? NormalizedStatusNameDPV { get; set; }

    [JsonPropertyName("standardizedStatusNameMatchSource")]
    public string? StandardizedStatusNameMatchSource { get; set; }

    [JsonPropertyName("resolutionMethodName")]
    public string? ResolutionMethodName { get; set; }

    [JsonPropertyName("ruralRouteHighwayContract")]
    public bool? RuralRouteHighwayContract { get; set; }

    [JsonPropertyName("generalDelivery")]
    public bool? GeneralDelivery { get; set; }

    [JsonPropertyName("attributes")]
    public Dictionary<string, object> Attributes { get; set; } = new ();
}
