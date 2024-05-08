using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Response;

public class RatedShipmentDetail
{
    [JsonPropertyName("rateType")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? RateType { get; set; }

    [JsonPropertyName("ratedWeightMethod")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? RatedWeightMethod { get; set; }

    [JsonPropertyName("totalDiscounts")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? TotalDiscounts { get; set; }

    [JsonPropertyName("totalBaseCharge")]
    public double? TotalBaseCharge { get; set; }

    [JsonPropertyName("totalNetCharge")]
    public double TotalNetCharge { get; set; } = 0;

    [JsonPropertyName("totalVatCharge")]
    public double? TotalVatCharge { get; set; }

    [JsonPropertyName("totalNetFedExCharge")]
    public double? TotalNetFedExCharge { get; set; }

    [JsonPropertyName("totalDutiesAndTaxes")]
    public double? TotalDutiesAndTaxes { get; set; }

    [JsonPropertyName("totalNetChargeWithDutiesAndTaxes")]
    public double? TotalNetChargeWithDutiesAndTaxes { get; set; }

    [JsonPropertyName("totalDutiesTaxesAndFees")]
    public double? TotalDutiesTaxesAndFees { get; set; }

    [JsonPropertyName("totalAncillaryFeesAndTaxes")]
    public double? TotalAncillaryFeesAndTaxes { get; set; }

    [JsonPropertyName("shipmentRateDetail")]
    public ShipmentRateDetail? ShipmentRateDetail { get; set; }

    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

    [JsonPropertyName("ratedPackages")]
    public List<RatedPackage>? RatedPackages { get; set; }

    [JsonPropertyName("anonymouslyAllowable")]
    public bool? AnonymouslyAllowable { get; set; }

    [JsonPropertyName("operationalDetail")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public OperationalDetail? OperationalDetail { get; set; }

    [JsonPropertyName("signatureOptionType")]
    public string? SignatureOptionType { get; set; }

    [JsonPropertyName("serviceDescription")]
    public ServiceDescription? ServiceDescription { get; set; }
}
