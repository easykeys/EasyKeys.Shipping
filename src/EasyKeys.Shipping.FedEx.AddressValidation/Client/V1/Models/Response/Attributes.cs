using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.AddressValidation.Client.V1.Models.Response;

public class Attributes
{
    [JsonPropertyName("POBox")]
    public bool? POBox { get; set; }

    [JsonPropertyName("POBoxOnlyZIP")]
    public bool? POBoxOnlyZIP { get; set; }

    [JsonPropertyName("SplitZip")]
    public bool? SplitZip { get; set; }

    [JsonPropertyName("SuiteRequiredButMissing")]
    public bool? SuiteRequiredButMissing { get; set; }

    [JsonPropertyName("InvalidSuiteNumber")]
    public bool? InvalidSuiteNumber { get; set; }

    [JsonPropertyName("ResolutionInput")]
    public string? ResolutionInput { get; set; }

    [JsonPropertyName("DPV")]
    public bool? DPV { get; set; }

    [JsonPropertyName("ResolutionMethod")]
    public string? ResolutionMethod { get; set; }

    [JsonPropertyName("DataVintage")]
    public string? DataVintage { get; set; }

    [JsonPropertyName("MatchSource")]
    public string? MatchSource { get; set; }

    [JsonPropertyName("CountrySupported")]
    public bool? CountrySupported { get; set; }

    [JsonPropertyName("ValidlyFormed")]
    public bool? ValidlyFormed { get; set; }

    [JsonPropertyName("Matched")]
    public bool? Matched { get; set; }

    [JsonPropertyName("Resolved")]
    public bool? Resolved { get; set; }

    [JsonPropertyName("Inserted")]
    public bool? Inserted { get; set; }

    [JsonPropertyName("MultiUnitBase")]
    public bool? MultiUnitBase { get; set; }

    [JsonPropertyName("ZIP11Match")]
    public bool? ZIP11Match { get; set; }

    [JsonPropertyName("ZIP4Match")]
    public bool? ZIP4Match { get; set; }

    [JsonPropertyName("UniqueZIP")]
    public bool? UniqueZIP { get; set; }

    [JsonPropertyName("StreetAddress")]
    public bool? StreetAddress { get; set; }

    [JsonPropertyName("RRConversion")]
    public bool? RRConversion { get; set; }

    [JsonPropertyName("ValidMultiUnit")]
    public bool? ValidMultiUnit { get; set; }

    [JsonPropertyName("AddressType")]
    public string? AddressType { get; set; }

    [JsonPropertyName("AddressPrecision")]
    public string? AddressPrecision { get; set; }

    [JsonPropertyName("MultipleMatches")]
    public bool? MultipleMatches { get; set; }
}
