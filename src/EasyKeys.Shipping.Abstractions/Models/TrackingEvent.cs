using EasyKeys.Shipping.Abstractions.Models;

public class TrackingEvent
{
    public DateTime TimeStamp { get; set; }

    public string Event { get; set; } = string.Empty;

    public string SignedBy { get; set; } = string.Empty;

    public Address Address { get; set; } = new Address();
}
