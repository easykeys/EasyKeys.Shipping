using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;

public class PackageSpecialServices
{
    [JsonPropertyName("specialServiceTypes")]
    public List<string>? SpecialServiceTypes { get; set; }

    [JsonPropertyName("signatureOptionType")]
    public List<string>? SignatureOptionType { get; set; }

    [JsonPropertyName("alcoholDetail")]
    public AlcoholDetail? AlcoholDetail { get; set; }

    [JsonPropertyName("dangerousGoodsDetail")]
    public DangerousGoodsDetail? DangerousGoodsDetail { get; set; }

    [JsonPropertyName("packageCODDetail")]
    public PackageCODDetail? PackageCODDetail { get; set; }

    [JsonPropertyName("pieceCountVerificationBoxCount")]
    public int? PieceCountVerificationBoxCount { get; set; }

    [JsonPropertyName("batteryDetails")]
    public List<BatteryDetail>? BatteryDetails { get; set; }

    [JsonPropertyName("dryIceWeight")]
    public DryIceWeight? DryIceWeight { get; set; }
}
