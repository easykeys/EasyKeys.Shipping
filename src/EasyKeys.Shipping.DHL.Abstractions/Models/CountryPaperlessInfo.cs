using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.DHL.Abstractions.Models;

public class CountryPaperlessInfo
{
    [JsonPropertyName("COUNTRY_CODE")]
    public string CountryCode { get; set; } = string.Empty;

    [JsonPropertyName("PAPERLESS_AVAILABLE")]
    public bool PaperlessAvailable { get; set; }

    [JsonPropertyName("VALUE_LIMIT")]
    public double? ValueLimit { get; set; }
}
