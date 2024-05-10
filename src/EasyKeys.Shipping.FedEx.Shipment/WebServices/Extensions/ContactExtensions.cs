using EasyKeys.Shipping.Abstractions.Models;

namespace EasyKeys.Shipping.FedEx.Shipment.WebServices.Extensions;

public static class ContactExtensions
{
    public static ShipClient.v25.Contact Map(this ContactInfo contact)
    {
        return new ShipClient.v25.Contact
        {
            CompanyName = contact.Company,
            PersonName = contact.FullName,
            PhoneNumber = contact.PhoneNumber,
            EMailAddress = contact.Email,
        };
    }
}
