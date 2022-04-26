using EasyKeys.Shipping.Abstractions.Models;

namespace EasyKeys.Shipping.Stamps.AddressValidation
{
    public interface IStampsAddressValidationProvider
    {
        /// <summary>
        /// Addresses cannot be verified for international shipments but we do verify the destination country. Please ensure
        /// that you use a country name from the accepted list published here ‐ <see href="http://pe.usps.com/text/imm/immctry.htm"/>.
        /// </summary>
        Task<ValidateAddress> ValidateAddressAsync(ValidateAddress validateAddress, CancellationToken cancellationToken);
    }
}
