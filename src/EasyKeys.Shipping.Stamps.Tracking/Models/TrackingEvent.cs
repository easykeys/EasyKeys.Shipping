using EasyKeys.Shipping.Abstractions.Models;

namespace EasyKeys.Shipping.Stamps.Tracking.Models
{
    public class TrackingEvent
    {
        public DateTime TimeStamp { get; set; }

        public string Event { get; set; } = string.Empty;

        public string SignedBy { get; set; } = string.Empty;

        public Address Address { get; set; } = new Address();

        public List<string> InternalErrors { get; set; } = new List<string>();
    }
}
