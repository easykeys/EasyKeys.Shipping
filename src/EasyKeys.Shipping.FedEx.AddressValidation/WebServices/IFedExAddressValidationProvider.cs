using EasyKeys.Shipping.Abstractions.Models;

namespace EasyKeys.Shipping.FedEx.AddressValidation.WebServices;

/// <summary>
/// Caution: FedEx Web Services Tracking, Address Validation, and Validate Postal Codes WSDLS will be disabled on August 31, 2024. The SOAP based FedEx Web Services is in development containment and has been replaced with FedEx RESTful APIs. To learn more and upgrade your integration from Web Services to FedEx APIs, please visit the FedEx Developer Portal.
/// </summary>
public interface IFedExAddressValidationProvider
{
    /// <summary>
    /// Validates USA based address and other countries, please refer to the manual. Uses Web Services Client.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ValidateAddress> ValidateAddressAsync(ValidateAddress request, CancellationToken cancellationToken = default);
}
