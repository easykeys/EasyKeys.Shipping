using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;

public class RequestedShipment
{
    [JsonPropertyName("shipper")]
    public Shipper? Shipper { get; set; }

    [JsonPropertyName("recipient")]
    public Recipient? Recipient { get; set; }

    [JsonPropertyName("serviceType")]
    public string? ServiceType { get; set; }

    [JsonPropertyName("emailNotificationDetail")]
    public EmailNotificationDetail? EmailNotificationDetail { get; set; }

    [JsonPropertyName("preferredCurrency")]
    public string? PreferredCurrency { get; set; }

    [JsonPropertyName("rateRequestType")]
    public List<string>? RateRequestType { get; set; }

    [JsonPropertyName("shipDateStamp")]
    public string? ShipDateStamp { get; set; }

    [JsonPropertyName("pickupType")]
    public string? PickupType { get; set; }

    [JsonPropertyName("requestedPackageLineItems")]
    public List<RequestedPackageLineItem>? RequestedPackageLineItems { get; set; }

    [JsonPropertyName("documentShipment")]
    public bool? DocumentShipment { get; set; }

    [JsonPropertyName("variableHandlingChargeDetail")]
    public VariableHandlingChargeDetail? VariableHandlingChargeDetail { get; set; }

    [JsonPropertyName("packagingType")]
    public string? PackagingType { get; set; }

    [JsonPropertyName("totalPackageCount")]
    public int? TotalPackageCount { get; set; }

    [JsonPropertyName("totalWeight")]
    public double? TotalWeight { get; set; }

    [JsonPropertyName("shipmentSpecialServices")]
    public ShipmentSpecialServices? ShipmentSpecialServices { get; set; }

    [JsonPropertyName("customsClearanceDetail")]
    public CustomsClearanceDetail? CustomsClearanceDetail { get; set; }

    [JsonPropertyName("groupShipment")]
    public bool? GroupShipment { get; set; }

    [JsonPropertyName("serviceTypeDetail")]
    public ServiceTypeDetail? ServiceTypeDetail { get; set; }

    [JsonPropertyName("smartPostInfoDetail")]
    public SmartPostInfoDetail? SmartPostInfoDetail { get; set; }

    [JsonPropertyName("expressFreightDetail")]
    public ExpressFreightDetail? ExpressFreightDetail { get; set; }

    [JsonPropertyName("groundShipment")]
    public bool? GroundShipment { get; set; }
}
