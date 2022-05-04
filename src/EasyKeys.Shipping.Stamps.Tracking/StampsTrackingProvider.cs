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
            _stampsClient = stampsClient;
        }

        public async Task<TrackingInformation> TrackShipmentAsync(ShipmentLabel shipmentLabel, CancellationToken cancellationToken)
        {
            var trackingInformation = new TrackingInformation();

            var trackRequest = new TrackShipmentRequest()
            {
                Item = await _stampsClient.GetTokenAsync(cancellationToken),
                Item1 = shipmentLabel.Labels.FirstOrDefault().TrackingId,
                Carrier = Carrier.All
            };

            try
            {
                var client = _stampsClient.CreateClient();

                var trackingResults = await client.TrackShipmentAsync(trackRequest);

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
