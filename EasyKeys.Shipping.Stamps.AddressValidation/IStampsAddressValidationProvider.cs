using EasyKeys.Shipping.Abstractions.Models;

namespace EasyKeys.Shipping.Stamps.AddressValidation
{
    public interface IStampsAddressValidationProvider
    {
        Task<Shipment> ValidateAddressAsync(Shipment shipmet, CancellationToken cancellationToken);
    }
}
