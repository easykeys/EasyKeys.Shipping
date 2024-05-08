using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;

public class ReturnShipmentDetail
{
    [JsonPropertyName("returnType")]
    public string? ReturnType { get; set; }
}
