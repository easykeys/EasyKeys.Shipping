using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Response;

public class Output
{
    [JsonPropertyName("rateReplyDetails")]
    public List<RateReplyDetail>? RateReplyDetails { get; set; }

    [JsonPropertyName("quoteDate")]
    public string? QuoteDate { get; set; }

    [JsonPropertyName("encoded")]
    public bool? Encoded { get; set; }

    [JsonPropertyName("alerts")]
    public List<Alert>? Alerts { get; set; }
}
