namespace EasyKeys.Shipping.FedEx.Shipment.Models;

public class EmailNotification
{
    /// <summary>
    /// <para>On_Delivery.</para>
    /// <para>On_Estimated_Delivery.</para>
    /// <para>On_Exception.</para>
    /// <para>On_Pickup_Driver_Arrived.</para>
    /// <para>On_Pickup_Driver_Assigned.</para>
    /// <para>On_Pickup_Driver_Departed.</para>
    /// <para>On_Pickup_Driver_En_Route.</para>
    /// <para>On_Shipment.</para>
    /// <para>On_Tender.</para>
    /// </summary>
    public IList<FedExNotificationEventType> EmailNotificationTypes { get; set; } = new List<FedExNotificationEventType>
    {
       FedExNotificationEventType.OnShipment,
       FedExNotificationEventType.OnDelivery,
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
