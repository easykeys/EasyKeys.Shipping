using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Response;

public class PackageRateDetail
{
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
    public BillingWeight? BillingWeight { get; set; }

    [JsonPropertyName("totalFreightDiscounts")]
    public double? TotalFreightDiscounts { get; set; }

    [JsonPropertyName("surcharges")]
    public List<SurCharge>? Surcharges { get; set; }

    [JsonPropertyName("currency")]
    public string? Currency { get; set; }
}
