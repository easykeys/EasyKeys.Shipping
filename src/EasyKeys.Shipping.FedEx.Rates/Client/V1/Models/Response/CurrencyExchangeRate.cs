using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Response;

public class CurrencyExchangeRate
{
    [JsonPropertyName("fromCurrency")]
    public string? FromCurrency { get; set; }

    [JsonPropertyName("doubleoCurrency")]
    public string? DoubleoCurrency { get; set; }

    [JsonPropertyName("rate")]
    public double? Rate { get; set; }
}
