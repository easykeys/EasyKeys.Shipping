using EasyKeys.Shipping.Abstractions.Models;

namespace Minimal.Apis.Models;

public class StampsShipmentDto : ShipmentDto
{
    public ContactInfo Sender { get; set; } = new ContactInfo
    {
        FirstName = "EasyKeys.com",
        LastName = "Fulfillment Center",
        Company = "EasyKeys.com",
        Email = "info@EasyKeys.com",
        PhoneNumber = "877.839.5397"
    };

    public ContactInfo? Recipient { get; set; }
}
