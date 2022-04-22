namespace EasyKeys.Shipping.Abstractions.Models;

public class ShipmentContacts
{
    public ShipmentContacts(SenderContact sender, SenderContact recipient)
    {
        Sender = sender;
        Recipient = recipient;
    }

    public SenderContact Sender { get; set; } = new SenderContact();

    public SenderContact Recipient { get; set; } = new SenderContact();
}
