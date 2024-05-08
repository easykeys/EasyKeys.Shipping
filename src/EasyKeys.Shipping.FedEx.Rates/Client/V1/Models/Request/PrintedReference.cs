using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;

public class PrintedReference
{
    [JsonPropertyName("printedReferenceType")]
    public string? PrintedReferenceType { get; set; }

    [JsonPropertyName("value")]
    public string? Value { get; set; }
}
