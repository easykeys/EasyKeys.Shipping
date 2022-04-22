using EasyKeys.Shipping.Abstractions.Models;

namespace EasyKeys.Shipping.Stamps.AddressValidation
{
    public interface IStampsAddressValidationProvider
    {
        Task<ValidateAddress> ValidateAddressAsync(ValidateAddress validateAddress, CancellationToken cancellationToken);
    }
}
