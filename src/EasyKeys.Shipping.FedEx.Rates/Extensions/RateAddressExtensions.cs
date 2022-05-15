using EasyKeys.Shipping.Abstractions;
using EasyKeys.Shipping.Abstractions.Models;

namespace EasyKeys.Shipping.FedEx.Extensions;

public static class RateAddressExtensions
{
    /// <summary>
    /// Get FedEx API address from ShippingRates.Address.
    /// </summary>
    /// <param name="address"></param>
    /// <returns></returns>
    public static RateClient.v28.Address GetFedExAddress(this Address address)
    {
        address = address ?? throw new ArgumentNullException(nameof(address));

        var stateOrProvince = (!address.IsUnitedStatesAddress() && address?.StateOrProvince?.Trim()?.Length > 2)
            ? string.Empty
            : address?.StateOrProvince ?? string.Empty;

        return new RateClient.v28.Address
        {
            StreetLines = address.GetStreetLines(),
            City = address.City?.Trim(),
            StateOrProvinceCode = stateOrProvince,
            PostalCode = address.PostalCode?.Trim(),
            CountryCode = address.CountryCode?.Trim(),
            CountryName = address.GetCountryName(),
            Residential = address.IsResidential,
            ResidentialSpecified = address.IsResidential,
        };
    }
}
