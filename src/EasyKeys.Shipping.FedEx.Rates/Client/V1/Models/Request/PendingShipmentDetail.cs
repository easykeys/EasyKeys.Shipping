using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;

public class PendingShipmentDetail
{
    [JsonPropertyName("pendingShipmentType")]
    public string? PendingShipmentType { get; set; }

    [JsonPropertyName("processingOptions")]
    public ProcessingOptions? ProcessingOptions { get; set; }

    [JsonPropertyName("recommendedDocumentSpecification")]
    public RecommendedDocumentSpecification? RecommendedDocumentSpecification { get; set; }

    [JsonPropertyName("emailLabelDetail")]
    public EmailLabelDetail? EmailLabelDetail { get; set; }

    [JsonPropertyName("documentReferences")]
    public List<DocumentReference>? DocumentReferences { get; set; }

    [JsonPropertyName("expirationTimeStamp")]
    public string? ExpirationTimeStamp { get; set; }

    [JsonPropertyName("shipmentDryIceDetail")]
    public ShipmentDryIceDetail? ShipmentDryIceDetail { get; set; }
}
