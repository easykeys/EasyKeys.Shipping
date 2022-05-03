using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.FedEx.Abstractions.Options;
using EasyKeys.Shipping.FedEx.Abstractions.Services;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using TrackClient.v19;

namespace EasyKeys.Shipping.FedEx.Tracking
{
    public class FedExTrackingProvider : IFedExTrackingProvider
    {
        private FedExOptions _options;
        private readonly ILogger<FedExTrackingProvider> _logger;
        private readonly TrackPortType _fedExClient;

        public FedExTrackingProvider(
            IOptionsMonitor<FedExOptions> optionsMonitor,
            ILogger<FedExTrackingProvider> logger,
            IFedExClientService fedExClient)
        {
            _options = optionsMonitor.CurrentValue;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _fedExClient = fedExClient.CreateTrackClient() ?? throw new ArgumentNullException(nameof(fedExClient));
        }

        public async Task<trackResponse> TrackShipmentAsync(ShipmentLabel label, CancellationToken cancellation)
        {
            var trackingRequest = new TrackRequest()
            {
                WebAuthenticationDetail = new WebAuthenticationDetail
                {
                    UserCredential = new WebAuthenticationCredential
                    {
                        Key = _options.FedExKey,
                        Password = _options.FedExPassword
                    }
                },
                ClientDetail = new ClientDetail
                {
                    AccountNumber = _options.FedExAccountNumber,
                    MeterNumber = _options.FedExMeterNumber
                },
                TransactionDetail = new TransactionDetail()
                {
                    CustomerTransactionId = "Track By Number_v16",
                    Localization = new Localization()
                    {
                        LanguageCode = "EN",
                        LocaleCode = "US"
                    }
                },
                Version = new VersionId()
                {
                    ServiceId = "track",
                    Major = 16,
                    Intermediate = 0,
                    Minor = 0
                },
                SelectionDetails = new TrackSelectionDetail[1]
                {
                    new TrackSelectionDetail()
                    {
                        CarrierCode = CarrierCodeType.FDXE,
                        PackageIdentifier = new TrackPackageIdentifier()
                        {
                            Type = TrackIdentifierType.TRACKING_NUMBER_OR_DOORTAG,
                            Value = label.Labels.FirstOrDefault().TrackingId
                        },
                        ShipmentAccountNumber = string.Empty,
                        SecureSpodAccount = string.Empty,
                    },
                }
            };
            try
            {
                var trackingRequest2 = new trackRequest1()
                {
                    TrackRequest = trackingRequest
                };

                return await _fedExClient.trackAsync(trackingRequest2);
            }
            catch (Exception ex)
            {
                return new trackResponse();
            }
        }
    }
}
