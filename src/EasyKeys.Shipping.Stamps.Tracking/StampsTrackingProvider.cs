using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Services;

using StampsClient.v111;

namespace EasyKeys.Shipping.Stamps.Tracking
{
    public class StampsTrackingProvider : IStampsTrackingProvider
    {
        private readonly IStampsClientService _stampsClient;

        public StampsTrackingProvider(IStampsClientService stampsClient)
        {
            _stampsClient = stampsClient ?? throw new ArgumentNullException(nameof(stampsClient));
        }

        public async Task<TrackingInformation> TrackShipmentAsync(string trackingId, CancellationToken cancellationToken)
        {
            var trackingInformation = new TrackingInformation();

            var trackRequest = new TrackShipmentRequest()
            {
                Item1 = trackingId,
                Carrier = Carrier.All
            };

            try
            {
                var trackingResults = await _stampsClient.TrackShipmentAsync(trackRequest, cancellationToken);

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
