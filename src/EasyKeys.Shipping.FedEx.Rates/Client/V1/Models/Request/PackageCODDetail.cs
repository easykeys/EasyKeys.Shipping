using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;

public class PackageCODDetail
{
    [JsonPropertyName("codCollectionAmount")]
    public CodCollectionAmount? CodCollectionAmount { get; set; }

    [JsonPropertyName("codCollectionType")]
    public string? CodCollectionType { get; set; }
}
