using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;

public class HomeDeliveryPremiumDetail
{
    [JsonPropertyName("phoneNumber")]
    public PhoneNumber? PhoneNumber { get; set; }

    [JsonPropertyName("shipTimestamp")]
    public string? ShipTimestamp { get; set; }

    [JsonPropertyName("homedeliveryPremiumType")]
    public string? HomedeliveryPremiumType { get; set; }
}
