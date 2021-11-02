using EasyKeys.Shipping.Usps.Tracking.Models;

namespace EasyKeys.Shipping.Usps.Tracking
{
    public interface IUspsTrackingClient
    {
        Task<List<TrackInfo>> GetTrackInfoAsync(List<string> trackingNumbers, CancellationToken cancellationToken);

        Task<List<TrackInfo>> GetTrackInfoAsync(List<TrackID> input, CancellationToken cancellationToken);

        Task<TrackInfo> GetTrackInfoAsync(string trackingNumber, CancellationToken cancellationToken);
    }
}
