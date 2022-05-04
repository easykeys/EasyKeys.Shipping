using EasyKeys.Shipping.Abstractions.Models;

namespace EasyKeys.Shipping.Stamps.Tracking
{
    public interface IStampsTrackingProvider
    {
        Task<TrackingInformation> TrackShipmentAsync(ShipmentLabel shipmentLabel, CancellationToken cancellationToken);
    }
}
