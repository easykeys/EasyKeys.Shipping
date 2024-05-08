using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;

public class OptionsRequested
{
    [JsonPropertyName("options")]
    public List<string>? Options { get; set; }
}
