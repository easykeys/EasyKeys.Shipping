using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;

public class Options
{
    [JsonPropertyName("labelTextOption")]
    public string? LabelTextOption { get; set; }

    [JsonPropertyName("customerSuppliedLabelText")]
    public string? CustomerSuppliedLabelText { get; set; }
}
