using EasyKeys.Shipping.Abstractions;

namespace EasyKeys.Shipping.Stamps.Shipment.Models
{
    public class ShipmentRequestDetails
    {
        public CustomsInformation CustomsInformation { get; set; } = new CustomsInformation();

        public LabelOptions LabelOptions { get; set; } = new LabelOptions();

        public NotificationOptions NotificationOptions { get; set; } = new NotificationOptions();

        public Rate SelectedRate { get; set; }
    }
}
