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
    public DateTime? ShippingDate { get; set; }

    /// <summary>
    /// Preferred currency code, applies to FedEx only.
    /// </summary>
    public string PreferredCurrencyCode { get; set; } = string.Empty;

    /// <summary>
    /// • YOUR_PACKAGING
    /// • FEDEX_10KG_BOX
    /// • FEDEX_25KG_BOX
    /// • FEDEX_BOX
    /// • FEDEX_ENVELOPE
    /// • FEDEX_EXTRA_LARGE_BOX
    /// • FEDEX_LARGE_BOX
    /// • FEDEX_MEDIUM_BOX
    /// • FEDEX_PAK
    /// • FEDEX_SMALL_BOX
    /// • FEDEX_TUBE.
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
