using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;

public class Commodity
{
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("weight")]
    public Weight? Weight { get; set; }

    [JsonPropertyName("quantity")]
    public int? Quantity { get; set; }

    [JsonPropertyName("customsValue")]
    public CustomsValue? CustomsValue { get; set; }

    [JsonPropertyName("unitPrice")]
    public UnitPrice? UnitPrice { get; set; }

    [JsonPropertyName("numberOfPieces")]
    public int? NumberOfPieces { get; set; }

    [JsonPropertyName("countryOfManufacture")]
    public string? CountryOfManufacture { get; set; }

    [JsonPropertyName("quantityUnits")]
    public string? QuantityUnits { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("harmonizedCode")]
    public string? HarmonizedCode { get; set; }

    [JsonPropertyName("partNumber")]
    public string? PartNumber { get; set; }
}
