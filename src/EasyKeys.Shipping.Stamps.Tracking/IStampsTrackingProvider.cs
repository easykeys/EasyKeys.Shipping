using EasyKeys.Shipping.Abstractions.Models;

namespace EasyKeys.Shipping.Stamps.Tracking
{
    public interface IStampsTrackingProvider
    {
        Task<TrackingInformation> TrackShipmentAsync(string trackingId, CancellationToken cancellationToken);
    }
}
