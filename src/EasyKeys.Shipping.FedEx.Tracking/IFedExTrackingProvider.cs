using EasyKeys.Shipping.Abstractions.Models;

namespace EasyKeys.Shipping.FedEx.Tracking
{
    public interface IFedExTrackingProvider
    {
        Task<TrackingInformation> TrackShipmentAsync(ShipmentLabel label, CancellationToken cancellation);
    }
}
