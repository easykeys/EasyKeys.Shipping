using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Response;

public class RatedPackage
{
    [JsonPropertyName("groupNumber")]
    public double? GroupNumber { get; set; }

    [JsonPropertyName("effectiveNetDiscount")]
    public double? EffectiveNetDiscount { get; set; }

    [JsonPropertyName("packageRateDetail")]
    public PackageRateDetail? PackageRateDetail { get; set; }

    [JsonPropertyName("rateType")]
    public string? RateType { get; set; }

    [JsonPropertyName("ratedWeightMethod")]
    public string? RatedWeightMethod { get; set; }

    [JsonPropertyName("baseCharge")]
    public double? BaseCharge { get; set; }

    [JsonPropertyName("netFreight")]
    public double? NetFreight { get; set; }

    [JsonPropertyName("totalSurcharges")]
    public double? TotalSurcharges { get; set; }

    [JsonPropertyName("netFedExCharge")]
    public double? NetFedExCharge { get; set; }

    [JsonPropertyName("totalTaxes")]
    public double? TotalTaxes { get; set; }

    [JsonPropertyName("netCharge")]
    public double? NetCharge { get; set; }

    [JsonPropertyName("totalRebates")]
    public double? TotalRebates { get; set; }

    [JsonPropertyName("billingWeight")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public BillingWeight? BillingWeight { get; set; }

    [JsonPropertyName("totalFreightDiscounts")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? TotalFreightDiscounts { get; set; }

    [JsonPropertyName("surcharges")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<SurCharge>? Surcharges { get; set; }

    [JsonPropertyName("currency")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Currency { get; set; }
}
