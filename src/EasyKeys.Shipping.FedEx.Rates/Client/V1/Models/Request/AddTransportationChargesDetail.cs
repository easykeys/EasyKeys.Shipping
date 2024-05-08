using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;

public class AddTransportationChargesDetail
{
    [JsonPropertyName("rateType")]
    public string? RateType { get; set; }

    [JsonPropertyName("rateLevelType")]
    public string? RateLevelType { get; set; }

    [JsonPropertyName("chargeLevelType")]
    public string? ChargeLevelType { get; set; }

    [JsonPropertyName("chargeType")]
    public string? ChargeType { get; set; }
}
