using EasyKeys.Shipping.Abstractions;

namespace EasyKeys.Shipping.FedEx.Shipment.Extensions
{
    public static class ShipmentAddressExtensions
    {
        /// <summary>
        /// Get FedEx API address from Shipment.Address.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static RateClient.v28.Address GetFedExAddress(this Address address)
        {
            address = address ?? throw new ArgumentNullException(nameof(address));

            return new RateClient.v28.Address
            {
                StreetLines = address.GetStreetLines(),
                City = address.City?.Trim(),
                StateOrProvinceCode = address.StateOrProvince?.Trim(),
                PostalCode = address.PostalCode?.Trim(),
                CountryCode = address.CountryCode?.Trim(),
                CountryName = address.GetCountryName(),
                Residential = address.IsResidential,
                ResidentialSpecified = address.IsResidential,
            };
        }
    }
}
