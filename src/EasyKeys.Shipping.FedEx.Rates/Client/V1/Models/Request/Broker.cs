using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;

public class Broker
{
    [JsonPropertyName("broker")]
    public Broker? Broker2 { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("brokerCommitTimestamp")]
    public string? BrokerCommitTimestamp { get; set; }

    [JsonPropertyName("brokerCommitDayOfWeek")]
    public string? BrokerCommitDayOfWeek { get; set; }

    [JsonPropertyName("brokerLocationId")]
    public string? BrokerLocationId { get; set; }

    [JsonPropertyName("brokerAddress")]
    public BrokerAddress? BrokerAddress { get; set; }

    [JsonPropertyName("brokerToDestinationDays")]
    public int? BrokerToDestinationDays { get; set; }
}
