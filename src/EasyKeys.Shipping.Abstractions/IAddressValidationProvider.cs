using EasyKeys.Shipping.Abstractions.Models;

namespace EasyKeys.Shipping.Abstractions;

/// <summary>
/// Physical Address Validation.
/// </summary>
public interface IAddressValidationProvider
{
    string Name { get; }

    /// <summary>
    /// Address Validation Provider Interface.
    /// </summary>
    /// <param name="validateAddress"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ValidateAddress> ValidateAddressAsync(ValidateAddress validateAddress, CancellationToken cancellationToken = default);
}
