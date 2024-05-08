using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;

public class HazardousCommodity
{
    [JsonPropertyName("quantity")]
    public Quantity? Quantity { get; set; }

    [JsonPropertyName("innerReceptacles")]
    public List<InnerReceptacle>? InnerReceptacles { get; set; }

    [JsonPropertyName("options")]
    public Options? Options { get; set; }

    [JsonPropertyName("description")]
    public Description? Description { get; set; }
}
