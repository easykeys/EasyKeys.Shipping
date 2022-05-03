using EasyKeys.Shipping.Abstractions.Models;

using TrackClient.v19;

namespace EasyKeys.Shipping.FedEx.Tracking
{
    public interface IFedExTrackingProvider
    {
        Task<trackResponse> TrackShipmentAsync(ShipmentLabel label, CancellationToken cancellation);
    }
}
