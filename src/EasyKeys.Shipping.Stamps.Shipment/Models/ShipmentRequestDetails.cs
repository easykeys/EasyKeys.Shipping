using EasyKeys.Shipping.Abstractions.Models;

namespace EasyKeys.Shipping.Stamps.Shipment.Models
{
    public class ShipmentRequestDetails
    {
        public CustomsInformation CustomsInformation { get; set; } = new CustomsInformation();

        public LabelOptions LabelOptions { get; set; } = new LabelOptions();

        public NotificationOptions NotificationOptions { get; set; } = new NotificationOptions();

        public Rate SelectedRate { get; set; }

        /// <summary>
        /// The amount to declare for this shipment, in dollars and cents. Required for International.
        /// </summary>
        public decimal DeclaredValue { get; set; }
    }
}
