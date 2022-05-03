using EasyKeys.Shipping.Abstractions.Models;

namespace EasyKeys.Shipping.Stamps.Tracking
{
    public interface IStampsTrackingProvider
    {
        Task<List<Models.TrackingEvent>> TrackShipmentAsync(ShipmentLabel shipmentLabel, CancellationToken cancellationToken);
    }
}
