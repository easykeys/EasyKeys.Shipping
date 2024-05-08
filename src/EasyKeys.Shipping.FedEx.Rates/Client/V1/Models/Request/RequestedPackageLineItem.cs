using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;

public class RequestedPackageLineItem
{
    [JsonPropertyName("subPackagingType")]
    public string? SubPackagingType { get; set; }

    [JsonPropertyName("groupPackageCount")]
    public int? GroupPackageCount { get; set; }

    [JsonPropertyName("contentRecord")]
    public List<ContentRecord>? ContentRecord { get; set; }

    [JsonPropertyName("declaredValue")]
    public DeclaredValue? DeclaredValue { get; set; }

    [JsonPropertyName("weight")]
    public Weight? Weight { get; set; }

    [JsonPropertyName("dimensions")]
    public Dimensions? Dimensions { get; set; }

    [JsonPropertyName("variableHandlingChargeDetail")]
    public VariableHandlingChargeDetail? VariableHandlingChargeDetail { get; set; }

    [JsonPropertyName("packageSpecialServices")]
    public PackageSpecialServices? PackageSpecialServices { get; set; }
}
