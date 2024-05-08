using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;

public class RequestRoot
{
    [JsonPropertyName("accountNumber")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public AccountNumber? AccountNumber { get; set; }

    [JsonPropertyName("rateRequestControlParameters")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public RateRequestControlParameters? RateRequestControlParameters { get; set; }

    [JsonPropertyName("requestedShipment")]
    required public RequestedShipment RequestedShipment { get; set; }

    [JsonPropertyName("carrierCodes")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string>? CarrierCodes { get; set; }
}
