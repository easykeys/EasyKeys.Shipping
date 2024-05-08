using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Response;

public class ShipmentRateDetail
{
    [JsonPropertyName("rateZone")]
    public string? RateZone { get; set; }

    [JsonPropertyName("dimDivisor")]
    public double? DimDivisor { get; set; }

    [JsonPropertyName("fuelSurchargePercent")]
    public double? FuelSurchargePercent { get; set; }

    [JsonPropertyName("totalSurcharges")]
    public double? TotalSurcharges { get; set; }

    [JsonPropertyName("totalFreightDiscount")]
    public double? TotalFreightDiscount { get; set; }

    [JsonPropertyName("surCharges")]
    public List<SurCharge>? SurCharges { get; set; }

    [JsonPropertyName("pricingCode")]
    public string? PricingCode { get; set; }

    [JsonPropertyName("currencyExchangeRate")]
    public CurrencyExchangeRate? CurrencyExchangeRate { get; set; }

    [JsonPropertyName("totalBillingWeight")]
    public TotalBillingWeight? TotalBillingWeight { get; set; }

    [JsonPropertyName("currency")]
    public string? Currency { get; set; }
}
