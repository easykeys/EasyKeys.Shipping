using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;

public class LocationContactAndAddress
{
    [JsonPropertyName("address")]
    public Address? Address { get; set; }

    [JsonPropertyName("contact")]
    public Contact? Contact { get; set; }
}
