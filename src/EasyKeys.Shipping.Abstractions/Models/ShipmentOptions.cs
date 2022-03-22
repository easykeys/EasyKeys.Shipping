namespace EasyKeys.Shipping.Abstractions.Models
{
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
        public string PreferredCurrencyCode { get; set; }

        /// <summary>
        /// Can be different packaging types.
        /// </summary>
        public string PackagingType { get; set; }

        /// <summary>
        /// Type of drop off, default is regular pickup.
        /// </summary>
        public string DropOffType { get; set; } = "RegularPickup";
    }
}
