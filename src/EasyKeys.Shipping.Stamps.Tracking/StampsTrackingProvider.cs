
using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Services;

using StampsClient.v111;

namespace EasyKeys.Shipping.Stamps.Tracking
{
    public class StampsTrackingProvider : IStampsTrackingProvider
    {
        private readonly IStampsClientService _stampsClient;
        private readonly IPolicyService _policy;

        public StampsTrackingProvider(IStampsClientService stampsClient, IPolicyService policy)
        {
            _stampsClient = stampsClient;
            _policy = policy;
        }

        public async Task<TrackingInformation> TrackShipmentAsync(string trackingId, CancellationToken cancellationToken)
        {
            var client = _stampsClient.CreateClient();

            var trackingInformation = new TrackingInformation();

            var trackRequest = new TrackShipmentRequest()
            {
                Item = await _stampsClient.GetTokenAsync(cancellationToken),
                Item1 = trackingId,
                Carrier = Carrier.All
            };

            try
            {
                var trackingResults = await _policy.GetRetryWithRefreshToken(cancellationToken).ExecuteAsync(async () => await client.TrackShipmentAsync(trackRequest));

                _stampsClient.SetToken(trackingResults.Authenticator);

                foreach (var trackingEvent in trackingResults.TrackingEvents)
                {
                    var address = new Shipping.Abstractions.Models.Address(trackingEvent.City, trackingEvent.State, trackingEvent.Zip, trackingEvent.Country);
                    trackingInformation.TrackingEvents.Add(new TrackingEvent()
                    {
                        Event = trackingEvent.Event,
                        TimeStamp = trackingEvent.Timestamp,
                        SignedBy = trackingEvent.SignedBy,
                        Address = address,
                    });
                }
            }
            catch (Exception ex)
            {
                trackingInformation.InternalErrors.Add(ex.Message);
            }

            return trackingInformation;
        }
    }
}
