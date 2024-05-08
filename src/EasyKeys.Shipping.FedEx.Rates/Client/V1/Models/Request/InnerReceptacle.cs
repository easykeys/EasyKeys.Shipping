using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;

public class InnerReceptacle
{
    [JsonPropertyName("quantity")]
    public Quantity? Quantity { get; set; }
}
