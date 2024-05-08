using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;

public class Broker2
{
    [JsonPropertyName("accountNumber")]
    public AccountNumber? AccountNumber { get; set; }

    [JsonPropertyName("address")]
    public object? Address { get; set; }

    [JsonPropertyName("contact")]
    public object? Contact { get; set; }
}
