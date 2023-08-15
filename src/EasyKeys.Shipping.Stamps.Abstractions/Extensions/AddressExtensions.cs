using EasyKeys.Shipping.Abstractions.Models;

namespace EasyKeys.Shipping.Stamps.Abstractions.Extensions;

public static class AddressExtensions
{
    public static StampsClient.v111.Address Map(this Address address, ContactInfo contactInfo)
    {
        return new StampsClient.v111.Address
        {
            FullName = contactInfo.FullName,
            FirstName = contactInfo.FirstName,
            LastName = contactInfo.LastName,
            PhoneNumber = contactInfo.PhoneNumber,
            EmailAddress = contactInfo.Email,
            Company = contactInfo.Company,
            Address1 = address.StreetLine,
            Address2 = address.StreetLine2,
            City = address.City,
            Province = address.IsUnitedStatesAddress() ? null : address.StateOrProvince,
            State = address.IsUnitedStatesAddress() ? address.StateOrProvince : null,
            Country = address.CountryCode,
            PostalCode = address.IsUnitedStatesAddress() ? null : address.PostalCode,
            ZIPCode = address.IsUnitedStatesAddress() ? address.PostalCode : null,
        };
    }
}
