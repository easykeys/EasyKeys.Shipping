using EasyKeys.Shipping.Abstractions;
using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.DHL.AddressValidation;

namespace EasyKeys.Shipping.DHL.AddressValidation;

public class DHLAddressValidationProvider : IAddressValidationProvider
{
    public string Name => throw new NotImplementedException();

    public Task<ValidateAddress> ValidateAddressAsync(ValidateAddress validateAddress, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
