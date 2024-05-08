using System.Text.Json.Serialization;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;

public class Recipient2
{
    [JsonPropertyName("emailAddress")]
    public string? EmailAddress { get; set; }

    [JsonPropertyName("notificationEventType")]
    public List<string>? NotificationEventType { get; set; }

    [JsonPropertyName("smsDetail")]
    public SmsDetail? SmsDetail { get; set; }

    [JsonPropertyName("notificationFormatType")]
    public string? NotificationFormatType { get; set; }

    [JsonPropertyName("emailNotificationRecipientType")]
    public string? EmailNotificationRecipientType { get; set; }

    [JsonPropertyName("notificationType")]
    public string? NotificationType { get; set; }

    [JsonPropertyName("locale")]
    public string? Locale { get; set; }

    [JsonPropertyName("optionsRequested")]
    public OptionsRequested? OptionsRequested { get; set; }

    [JsonPropertyName("role")]
    public string? Role { get; set; }
}
