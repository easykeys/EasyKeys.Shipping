using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;

public class InternationalControlledExportDetail
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }
}
