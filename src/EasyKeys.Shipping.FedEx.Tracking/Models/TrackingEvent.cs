
using EasyKeys.Shipping.Abstractions.Models;

namespace EasyKeys.Shipping.FedEx.Tracking.Models
{
    public class TrackingEvent
    {
        public DateTime TimeStamp { get; set; }

        public string EventType { get; set; } = string.Empty;

        public string EventDescription { get; set; } = string.Empty;

        public string StatusExceptionCode { get; set; } = string.Empty;

        public string StatusExceptionDescription { get; set; } = string.Empty;

        public Address Address { get; set; } = new Address();

        public string ArrivalLocation { get; set; } = string.Empty;
    }
}
