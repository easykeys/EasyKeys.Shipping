namespace EasyKeys.Shipping.Stamps.AddressValidation.Extensions;

public static class RateAddressExtensions
{
    public static StampsClient.v111.Address GetStampsAddress(this Shipping.Abstractions.Models.Address address)
    {
        address = address ?? throw new ArgumentNullException(nameof(address));

        return new StampsClient.v111.Address
        {
            FullName = "This is required for address validation",
            Address1 = address.StreetLine,
            Address2 = address.StreetLine2,
            City = address.City,
            State = address.IsUnitedStatesAddress() ? address.StateOrProvince : null,
            Province = !address.IsUnitedStatesAddress() ? address.StateOrProvince : null,
            ZIPCode = address.IsUnitedStatesAddress() ? address.PostalCode.Substring(0, 5) : null,
            PostalCode = (!address.IsUnitedStatesAddress()) ? address.PostalCode : null,
            Country = address.GetCountryCode(),
        };
    }
}
