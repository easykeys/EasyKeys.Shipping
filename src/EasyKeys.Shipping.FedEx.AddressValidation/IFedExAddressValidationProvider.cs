using System.Threading;
using System.Threading.Tasks;

using EasyKeys.Shipping.Abstractions.Models;

namespace EasyKeys.Shipping.FedEx.AddressValidation
{
    public interface IFedExAddressValidationProvider
    {
        Task<ValidateAddress> ValidateAddressAsync(ValidateAddress request, CancellationToken cancellationToken = default);
    }
}
