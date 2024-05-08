using EasyKeys.Shipping.FedEx.AddressValidation.Client.V1.Models.Request;
using EasyKeys.Shipping.FedEx.AddressValidation.Client.V1.Models.Response;

namespace EasyKeys.Shipping.FedEx.AddressValidation.Client.V1;

/// <summary>
/// Implmentation of the FedEx Address Validation Client.
/// <see href="https://developer.fedex.com/api/en-us/catalog/address-validation/v1/docs.html"/>.
/// </summary>
public interface IFedexAddressValidationClient
{
    /// <summary>
    /// Validates an address asynchronously.
    /// </summary>
    /// <param name="request">The address validation request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The address validation response.</returns>
    Task<ResponseRoot> ValidateAddressAsync(RequestRoot request, CancellationToken cancellationToken = default);
}
