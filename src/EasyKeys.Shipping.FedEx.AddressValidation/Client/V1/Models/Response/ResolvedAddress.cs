namespace EasyKeys.Shipping.FedEx.AddressValidation.Client.V1.Models.Response;

public class ResolvedAddress
{
    public string[]? StreetLinesToken { get; set; }

    public string? City { get; set; }

    public string? StateOrProvinceCode { get; set; }

    public string? CountryCode { get; set; }

    public List<CustomerMessage>? CustomerMessage { get; set; }

    public List<ResolutionToken>? CityToken { get; set; }

    public ResolutionToken? PostalCodeToken { get; set; }

    public ParsedPostalCode? ParsedPostalCode { get; set; }

    public string? Classification { get; set; }

    public bool PostOfficeBox { get; set; }

    public bool NormalizedStatusNameDPV { get; set; }

    public string? StandardizedStatusNameMatchSource { get; set; }

    public string? ResolutionMethodName { get; set; }

    public bool RuralRouteHighwayContract { get; set; }

    public bool GeneralDelivery { get; set; }

    public Dictionary<string, object> Attributes { get; set; } = new ();
}
