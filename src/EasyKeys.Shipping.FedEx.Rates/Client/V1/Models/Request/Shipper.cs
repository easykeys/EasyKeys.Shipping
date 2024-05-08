using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;

public class Shipper
{
    [JsonPropertyName("address")]
    public Address? Address { get; set; }
}
