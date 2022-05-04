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

        public async Task<TrackingInformation> TrackShipmentAsync(ShipmentLabel label, CancellationToken cancellation)
        {
            var trackingInformation = new TrackingInformation() { TrackingEvents = new List<TrackingEvent>() };

            var trackingRequest = new trackRequest1()
            {
                TrackRequest = new TrackRequest()
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
                        ServiceId = "trck",
                        Major = 19,
                        Intermediate = 0,
                        Minor = 0
                    },
                    SelectionDetails = new TrackSelectionDetail[]
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
                }
            };

            try
            {
                var trackingReply = await _fedExClient.trackAsync(trackingRequest);

                if ((trackingReply.TrackReply?.HighestSeverity != NotificationSeverityType.ERROR)
                        && (trackingReply.TrackReply?.HighestSeverity != NotificationSeverityType.FAILURE))
                {
                    var events = trackingReply?.TrackReply?.CompletedTrackDetails.SelectMany(x => x.TrackDetails)
                        .SelectMany(x => x.Events);

                    if (events == null)
                    {
                        trackingInformation.InternalErrors.Add("No Tracking events available");
                        return trackingInformation;
                    }

                    foreach (var trackingEvent in events)
                    {
                        trackingInformation.TrackingEvents.Add(new TrackingEvent()
                        {
                            Event = trackingEvent.EventDescription,
                            TimeStamp = trackingEvent.Timestamp,
                            Address = new Shipping.Abstractions.Models.Address(
                                                                                trackingEvent.Address.City,
                                                                                trackingEvent.Address.StateOrProvinceCode,
                                                                                trackingEvent.Address.PostalCode,
                                                                                trackingEvent.Address.CountryCode),
                        });
                    }
                }
                else
                {
                    trackingInformation.InternalErrors.Add(trackingReply.TrackReply.Notifications.Select(x => x.Message).Flatten(","));
                }
            }
            catch (Exception ex)
            {
                trackingInformation.InternalErrors.Add($"FedEx provider exception: {ex.Message}");
            }

            return trackingInformation;
        }
    }
}
