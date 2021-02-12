using System.Threading.Tasks;

using EasyKeys.Shipping.FedEx.AddressValidation.Models;

namespace EasyKeys.Shipping.FedEx.AddressValidation
{
    public interface IValidationClient
    {
        Task<ValidationResponse> ValidateAddressAsync(ValidationRequest request);

        ValidationResponse InputValidation(ValidationRequest request);
    }
}
