using EasyKeys.Shipping.Abstractions;
using EasyKeys.Shipping.Abstractions.Models;

namespace EasyKeys.Shipping.FedEx.Rates.WebServices.Extensions;

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

        // https://github.com/nopSolutions/FedEx-plugin-for-nopcommerce/blob/4f65483b4463b85d5a0c70c7aa84ff9a7cb5fde6/Nop.Plugin.Shipping.Fedex/Services/FedexService.cs#L485
        var stateOrProvince = !address.IsUnitedStatesAddress() && address?.StateOrProvince?.Trim()?.Length > 2
            ? string.Empty
            : address?.StateOrProvince ?? string.Empty;

        return new RateClient.v28.Address
        {
            StreetLines = address?.GetStreetLines(),
            City = address?.City?.Trim(),
            StateOrProvinceCode = stateOrProvince,
            PostalCode = address?.PostalCode?.Trim(),
            CountryCode = address?.GetCountryCode().Trim(),
            CountryName = address?.GetCountryName(),
            Residential = address?.IsResidential ?? false,
            ResidentialSpecified = address?.IsResidential ?? false,
        };
    }
}
