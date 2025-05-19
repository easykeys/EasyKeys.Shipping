using EasyKeys.Shipping.Abstractions.Models;

namespace EasyKeys.Shipping.DHL.Abstractions;

public interface IDHLExpressAddressValidationProvider
{
    /// <summary>
    /// Validates if DHL Express has got pickup/delivery capabilities at origin/destination.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ValidateAddress> ValidateAddressAsync(ValidateAddress request, CancellationToken cancellationToken = default);
}
