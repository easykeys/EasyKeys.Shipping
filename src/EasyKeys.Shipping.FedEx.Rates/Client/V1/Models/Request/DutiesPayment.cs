using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;

public class DutiesPayment
{
    [JsonPropertyName("payor")]
    public Payor? Payor { get; set; }

    [JsonPropertyName("paymentType")]
    public string? PaymentType { get; set; }
}
