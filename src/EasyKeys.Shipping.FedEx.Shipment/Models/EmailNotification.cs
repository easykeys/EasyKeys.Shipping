namespace EasyKeys.Shipping.FedEx.Shipment.Models;

public class EmailNotification
{
    /// <summary>
    /// On_Delivery,
    /// On_Estimated_Delivery,
    /// On_Exception,
    /// On_Pickup_Driver_Arrived,
    /// On_Pickup_Driver_Assigned,
    /// On_Pickup_Driver_Departed,
    /// On_Pickup_Driver_En_Route,
    /// On_Shipment,On_Tender.
    /// </summary>
    public IList<string> EmailNotificationTypes { get; set; } = new List<string>
    {
       "On_Shipment",
       "On_Delivery"
    };

    /// <summary>
    /// Personal Message to be displayed in the email.
    /// </summary>
    public string PersonalMessage { get; set; } = "Order has been shipped";

    /// <summary>
    /// two-letter code for language (e.g. EN, FR, etc.).
    /// </summary>
    public string LanguageCode { get; set; } = "EN";

    /// <summary>
    /// html, text.
    /// </summary>
    public string NotificationFormatType { get; set; } = "HTML";

    /// <summary>
    /// BROKER,
    /// OTHER,
    /// RECIPIENT,
    /// SHIPPER,
    ///  THIRD_PARTY.
    /// </summary>
    public string NotificationRoleType { get; set; } = "Recipient";
}
