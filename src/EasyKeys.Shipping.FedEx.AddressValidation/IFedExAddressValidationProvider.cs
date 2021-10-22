using EasyKeys.Shipping.Abstractions.Models;

namespace EasyKeys.Shipping.FedEx.AddressValidation;

public interface IFedExAddressValidationProvider
{
    /// <summary>
    /// Validates USA based address and other countries, please refer to the manual.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ValidateAddress> ValidateAddressAsync(ValidateAddress request, CancellationToken cancellationToken = default);
}
