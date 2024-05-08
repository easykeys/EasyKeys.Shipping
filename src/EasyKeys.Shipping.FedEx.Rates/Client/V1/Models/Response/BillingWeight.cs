using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Response;

public class BillingWeight
{
    [JsonPropertyName("units")]
    public string? Units { get; set; }

    [JsonPropertyName("value")]
    public double? Value { get; set; }
}
