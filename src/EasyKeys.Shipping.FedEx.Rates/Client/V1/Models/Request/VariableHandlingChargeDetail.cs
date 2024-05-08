using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;

public class VariableHandlingChargeDetail
{
    [JsonPropertyName("rateType")]
    public string? RateType { get; set; }

    [JsonPropertyName("percentValue")]
    public int? PercentValue { get; set; }

    [JsonPropertyName("rateLevelType")]
    public string? RateLevelType { get; set; }

    [JsonPropertyName("fixedValue")]
    public FixedValue? FixedValue { get; set; }

    [JsonPropertyName("rateElementBasis")]
    public string? RateElementBasis { get; set; }
}
