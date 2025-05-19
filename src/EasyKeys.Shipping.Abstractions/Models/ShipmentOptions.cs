namespace EasyKeys.Shipping.Abstractions.Models;

public class ShipmentOptions
{
    public const string DefaultCurrencyCode = "USD";

    public ShipmentOptions(
        string packagingType,
        DateTime shippingDate,
        bool saturdayDelivery = true)
    {
        PackagingType = packagingType;
        ShippingDate = shippingDate;
        SaturdayDelivery = saturdayDelivery;
    }

    /// <summary>
    /// <para>Enable Saturday Delivery option for shipping rates.</para>
    /// <para>The default value is false.</para>
    /// </summary>
    public bool SaturdayDelivery { get; }

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
    /// Based on the provider.
    /// </summary>
    public string DropOffType { get; set; } = "REGULAR_PICKUP";

    /// <summary>
    /// Input fedex account number wished to be used.
    /// </summary>
    public string? CustomerFedexAccountNumber { get; set; } = null;

    /// <summary>
    /// Represents the DHL Express account number for a customer. It can be null if not provided.
    /// </summary>
    public string? CustomerDHLExpressAccountNumber { get; set; } = null;
}
