using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;

public class CommercialInvoice
{
    [JsonPropertyName("shipmentPurpose")]
    public string? ShipmentPurpose { get; set; }
}
