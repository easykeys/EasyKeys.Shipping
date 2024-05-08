using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;

public class EmailLabelDetail
{
    [JsonPropertyName("recipients")]
    public List<Recipient>? Recipients { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }
}
