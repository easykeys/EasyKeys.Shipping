using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;

public class DeliveryOnInvoiceAcceptanceDetail
{
    [JsonPropertyName("recipient")]
    public Recipient? Recipient { get; set; }
}
