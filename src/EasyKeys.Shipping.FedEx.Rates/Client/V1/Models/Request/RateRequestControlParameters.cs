using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;

public class RateRequestControlParameters
{
    [JsonPropertyName("returnTransitTimes")]
    public bool? ReturnTransitTimes { get; set; }

    [JsonPropertyName("servicesNeededOnRateFailure")]
    public bool? ServicesNeededOnRateFailure { get; set; }

    [JsonPropertyName("variableOptions")]
    public string? VariableOptions { get; set; }

    [JsonPropertyName("rateSortOrder")]
    public string? RateSortOrder { get; set; }
}
