using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EasyKeys.Shipping.Abstractions.Models;

namespace EasyKeys.Shipping.FedEx.AddressValidation.Api.V1;

/// <summary>
/// This validates recipient address information and identifies it as either business or residential.Uses Fedex REST Api Address Validation Client V1.
/// </summary>
public interface IFedExAddressValidationApiClient
{
    /// <summary>
    /// Validates USA based address and other countries, please refer to the manual.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ValidateAddress> ValidateAddressAsync(ValidateAddress request, CancellationToken cancellationToken = default);
}
