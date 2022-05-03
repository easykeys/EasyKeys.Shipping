
using AddressValidationClient.v4;

using EasyKeys.Shipping.FedEx.Abstractions.Options;

using Microsoft.Extensions.Options;

using RateClient.v28;

using ShipClient.v25;

using TrackClient.v19;

namespace EasyKeys.Shipping.FedEx.Abstractions.Services.Impl
{
    internal sealed class FedExClientService : IFedExClientService
    {
        private readonly FedExOptions _fedExOptions;

        public FedExClientService(IOptionsMonitor<FedExOptions> fedExOptions)
        {
            _fedExOptions = fedExOptions.CurrentValue;
        }

        public AddressValidationPortType CreateAddressValidationClient()
        {
            return new AddressValidationPortTypeClient(AddressValidationPortTypeClient.EndpointConfiguration.AddressValidationServicePort, _fedExOptions.Url);
        }

        public RatePortType CreateRateClient()
        {
            return new RatePortTypeClient(RatePortTypeClient.EndpointConfiguration.RateServicePort, _fedExOptions.Url);
        }

        public ShipPortType CreateShipClient()
        {
            return new ShipPortTypeClient(ShipPortTypeClient.EndpointConfiguration.ShipServicePort, _fedExOptions.Url);
        }

        public TrackPortType CreateTrackClient()
        {
            return new TrackPortTypeClient(TrackPortTypeClient.EndpointConfiguration.TrackServicePort, _fedExOptions.Url);
        }
    }
}
