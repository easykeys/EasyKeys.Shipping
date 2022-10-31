using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.FedEx.Abstractions.Options;
using EasyKeys.Shipping.FedEx.Abstractions.Services;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using TrackClient.v19;

namespace EasyKeys.Shipping.FedEx.Tracking;

public class FedExTrackingProvider : IFedExTrackingProvider
{
    private readonly ILogger<FedExTrackingProvider> _logger;
    private readonly TrackPortType _fedExClient;
    private FedExOptions _options;

    public FedExTrackingProvider(
        IOptionsMonitor<FedExOptions> optionsMonitor,
        ILogger<FedExTrackingProvider> logger,
        IFedExClientService fedExClient)
    {
        _options = optionsMonitor.CurrentValue;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _fedExClient = fedExClient.CreateTrackClient() ?? throw new ArgumentNullException(nameof(fedExClient));
    }

    public async Task<TrackingInformation> TrackShipmentAsync(string trackingId, CancellationToken cancellation)
    {
        var trackingInformation = new TrackingInformation()
        {
            TrackingEvents = new List<TrackingEvent>()
        };

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
                            Value = trackingId
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
                var events = trackingReply?.TrackReply?.CompletedTrackDetails?.SelectMany(x => x.TrackDetails)
                                                                             ?.SelectMany(x => x.Events) ?? new List<TrackEvent>();

                var d = trackingReply?.TrackReply?.Notifications.Select(x => x.Message);

                if (d?.Any() ?? false)
                {
                    trackingInformation.TrackingEvents.Add(new TrackingEvent()
                    {
                        Event = d.First(),
                        TimeStamp = DateTime.Now,
                        Address = new Shipping.Abstractions.Models.Address(),
                    });

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
                var errors = trackingReply.TrackReply.Notifications.Select(x => x.Message).Flatten(",");
                _logger.LogError("{providerName} failed: {errors}", nameof(FedExTrackingProvider), errors);

                trackingInformation.InternalErrors.Add(errors);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{providerName} failed", nameof(FedExTrackingProvider));

            trackingInformation.InternalErrors.Add(ex?.Message ?? $"{nameof(FedExTrackingProvider)} failed");
        }

        return trackingInformation;
    }
}
