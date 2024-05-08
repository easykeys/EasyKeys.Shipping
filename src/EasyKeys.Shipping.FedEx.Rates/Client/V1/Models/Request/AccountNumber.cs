using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;

public class AccountNumber
{
    [JsonPropertyName("value")]
    public string? Value { get; set; }
}
