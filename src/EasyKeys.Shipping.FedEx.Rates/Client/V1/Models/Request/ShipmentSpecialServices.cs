using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;

public class ShipmentSpecialServices
{
    [JsonPropertyName("returnShipmentDetail")]
    public ReturnShipmentDetail? ReturnShipmentDetail { get; set; }

    [JsonPropertyName("deliveryOnInvoiceAcceptanceDetail")]
    public DeliveryOnInvoiceAcceptanceDetail? DeliveryOnInvoiceAcceptanceDetail { get; set; }

    [JsonPropertyName("internationalTrafficInArmsRegulationsDetail")]
    public InternationalTrafficInArmsRegulationsDetail? InternationalTrafficInArmsRegulationsDetail { get; set; }

    [JsonPropertyName("pendingShipmentDetail")]
    public PendingShipmentDetail? PendingShipmentDetail { get; set; }

    [JsonPropertyName("holdAtLocationDetail")]
    public HoldAtLocationDetail? HoldAtLocationDetail { get; set; }

    [JsonPropertyName("shipmentCODDetail")]
    public ShipmentCODDetail? ShipmentCODDetail { get; set; }

    [JsonPropertyName("shipmentDryIceDetail")]
    public ShipmentDryIceDetail? ShipmentDryIceDetail { get; set; }

    [JsonPropertyName("internationalControlledExportDetail")]
    public InternationalControlledExportDetail? InternationalControlledExportDetail { get; set; }

    [JsonPropertyName("homeDeliveryPremiumDetail")]
    public HomeDeliveryPremiumDetail? HomeDeliveryPremiumDetail { get; set; }

    [JsonPropertyName("specialServiceTypes")]
    public List<string>? SpecialServiceTypes { get; set; }
}
