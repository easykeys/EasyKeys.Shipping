using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;

public class Description
{
    [JsonPropertyName("sequenceNumber")]
    public int? SequenceNumber { get; set; }

    [JsonPropertyName("processingOptions")]
    public List<string>? ProcessingOptions { get; set; }

    [JsonPropertyName("subsidiaryClasses")]
    public string? SubsidiaryClasses { get; set; }

    [JsonPropertyName("labelText")]
    public string? LabelText { get; set; }

    [JsonPropertyName("technicalName")]
    public string? TechnicalName { get; set; }

    [JsonPropertyName("packingDetails")]
    public PackingDetails? PackingDetails { get; set; }

    [JsonPropertyName("authorization")]
    public string? Authorization { get; set; }

    [JsonPropertyName("reportableQuantity")]
    public bool? ReportableQuantity { get; set; }

    [JsonPropertyName("percentage")]
    public int? Percentage { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("packingGroup")]
    public string? PackingGroup { get; set; }

    [JsonPropertyName("properShippingName")]
    public string? ProperShippingName { get; set; }

    [JsonPropertyName("hazardClass")]
    public string? HazardClass { get; set; }
}
