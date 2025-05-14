using EasyKeys.Shipping.Abstractions;
using EasyKeys.Shipping.Abstractions.Models;

namespace EasyKeys.Shipping.DHL.Abstractions;

public class DHLExpressAddressValidationProvider : IAddressValidationProvider
{
    public string Name => throw new NotImplementedException();

    public Task<ValidateAddress> ValidateAddressAsync(ValidateAddress validateAddress, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
