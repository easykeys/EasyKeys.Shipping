using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;

public class ExpressFreightDetail
{
    [JsonPropertyName("bookingConfirmationNumber")]
    public string? BookingConfirmationNumber { get; set; }

    [JsonPropertyName("shippersLoadAndCount")]
    public int? ShippersLoadAndCount { get; set; }
}
