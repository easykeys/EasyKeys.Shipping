using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;

public class RecommendedDocumentSpecification
{
    [JsonPropertyName("types")]
    public List<string>? Types { get; set; }
}
