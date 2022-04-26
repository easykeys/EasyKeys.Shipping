namespace EasyKeys.Shipping.Abstractions.Models;

public class ShipmentOptions
{
    public const string DefaultCurrencyCode = "USD";

    /// <summary>
    /// Enable Saturday Delivery option for shipping rates.
    /// </summary>
    public bool SaturdayDelivery { get; set; }

    /// <summary>
    /// Pickup date. Current date and time is used if not specified.
    /// </summary>
    public DateTime ShippingDate { get; set; } = DateTime.Now;

    /// <summary>
    /// Preferred currency code, applies to FedEx only.
    /// </summary>
    public string PreferredCurrencyCode { get; set; } = string.Empty;

    /// <summary>
    /// <b>FEDEX Shipping </b>
    /// <list type="bullet">
    /// <item>
    /// <description> YOUR_PACKAGING
    /// </description>
    /// </item>
    /// <item>
    /// <description>FEDEX_10KG_BOX
    /// </description>
    /// </item>
    /// <item>
    /// <description>FEDEX_25KG_BOX
    /// </description>
    /// </item>
    /// <item>
    /// <description>FEDEX_BOX
    /// </description>
    /// </item>
    /// <item>
    /// <description>FEDEX_ENVELOPE
    /// </description>
    /// </item>
    /// <item>
    /// <description>FEDEX_EXTRA_LARGE_BOX
    /// </description>
    /// </item>
    /// <item>
    /// <description>FEDEX_LARGE_BOX
    /// </description>
    /// </item>
    /// <item>
    /// <description>FEDEX_MEDIUM_BOX
    /// </description>
    /// </item>
    /// <item>
    /// <description>FEDEX_PAK
    /// </description>
    /// </item>
    /// <item>
    /// <description>FEDEX_SMALL_BOX
    /// </description>
    /// </item>
    /// <item>
    /// <description>FEDEX_TUBE.
    /// </description>
    /// </item>
    /// </list>
    /// </summary>
    public string PackagingType { get; set; } = "YOUR_PACKAGING";

    /// <summary>
    /// RegularPickup,
    /// DropBox,
    /// BusinessServiceCenter,
    /// RequestCourier,
    /// Station.
    /// </summary>
    public string DropOffType { get; set; } = "RegularPickup";
}
