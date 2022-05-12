namespace EasyKeys.Shipping.Abstractions.Models;

public class ShipmentOptions
{
    public const string DefaultCurrencyCode = "USD";

    public ShipmentOptions(string packagingType, DateTime shippingDate)
    {
        PackagingType = packagingType;
        ShippingDate = shippingDate;
    }

    /// <summary>
    /// <para>Enable Saturday Delivery option for shipping rates.</para>
    /// <para>The default value is false.</para>
    /// </summary>
    public bool SaturdayDelivery { get; set; }

    /// <summary>
    /// <para>Pickup date. Current date and time is used if not specified.</para>
    /// <para>The default value is DateTime.Now.</para>
    /// </summary>
    public DateTime ShippingDate { get; } = DateTime.Now;

    /// <summary>
    /// <para>Preferred currency code, applies to FedEx only.</para>
    /// <para>The default value is "USD".</para>
    /// </summary>
    public string PreferredCurrencyCode { get; set; } = DefaultCurrencyCode;

    /// <summary>
    /// <para>The type of the packing.</para>
    /// <para>FedEx example is YOUR_PACKAGING.</para>
    /// <para>USPS example is PACKAGE_SERVICE_RETAIL.</para>
    /// </summary>
    public string PackagingType { get; } = "YOUR_PACKAGING";

    /// <summary>
    /// RegularPickup,
    /// DropBox,
    /// BusinessServiceCenter,
    /// RequestCourier,
    /// Station.
    /// </summary>
    public string DropOffType { get; set; } = "RegularPickup";
}
