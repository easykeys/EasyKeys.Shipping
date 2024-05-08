using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;

public class EmailNotificationDetail
{
    [JsonPropertyName("recipients")]
    public List<Recipient>? Recipients { get; set; }

    [JsonPropertyName("personalMessage")]
    public string? PersonalMessage { get; set; }

    [JsonPropertyName("PrintedReference")]
    public PrintedReference? PrintedReference { get; set; }
}
