using EasyKeys.Shipping.Abstractions;
using EasyKeys.Shipping.Abstractions.Models;

namespace EasyKeys.Shipping.FedEx.Shipment.WebServices.Extensions;

public static class ShipmentAddressExtensions
{
    /// <summary>
    /// Get FedEx API address from Shipment.Address.
    /// </summary>
    /// <param name="address"></param>
    /// <returns></returns>
    public static ShipClient.v25.Address GetFedExAddress(this Address address)
    {
        address = address ?? throw new ArgumentNullException(nameof(address));

        return new ShipClient.v25.Address
        {
            StreetLines = address.GetStreetLines(),
            City = address.City?.Trim(),
            StateOrProvinceCode = address.StateOrProvince?.Trim(),
            PostalCode = address.PostalCode?.Trim(),
            CountryCode = address.GetCountryCode().Trim(),
            CountryName = address.GetCountryName(),
            Residential = address.IsResidential,
            ResidentialSpecified = address.IsResidential,
        };
    }
}
