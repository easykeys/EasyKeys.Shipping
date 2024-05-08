using System.Text.Json;

using Bet.Extensions;

using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.FedEx.Abstractions.Models;
using EasyKeys.Shipping.FedEx.AddressValidation;
using EasyKeys.Shipping.FedEx.Rates;
using EasyKeys.Shipping.FedEx.Shipment;
using EasyKeys.Shipping.FedEx.Shipment.Models;
using EasyKeys.Shipping.FedEx.Tracking;

using Models;

namespace EasyKeys.Shipping.FedEx.Console;

public class Main : IMain
{
    private readonly IFedExAddressValidationProvider _validationClient;
    private readonly IFedExRateProvider _fedexRateProvider;
    private readonly IFedExShipmentProvider _fedExShipmentProvider;
    private readonly IFedExTrackingProvider _fedExTrackingProvider;
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly ILogger<Main> _logger;

    public Main(
        IFedExAddressValidationProvider validationClient,
        IFedExRateProvider fedExRateProvider,
        IFedExShipmentProvider fedExShipmentProvider,
        IFedExTrackingProvider fedExTrackingProvider,
        IHostApplicationLifetime applicationLifetime,
        IConfiguration configuration,
        ILogger<Main> logger)
    {
        _validationClient = validationClient ?? throw new ArgumentNullException(nameof(validationClient));
        _fedexRateProvider = fedExRateProvider ?? throw new ArgumentNullException(nameof(fedExRateProvider));
        _fedExShipmentProvider = fedExShipmentProvider ?? throw new ArgumentNullException(nameof(fedExShipmentProvider));
        _applicationLifetime = applicationLifetime ?? throw new ArgumentNullException(nameof(applicationLifetime));
        _fedExTrackingProvider = fedExTrackingProvider ?? throw new ArgumentNullException(nameof(fedExTrackingProvider));
        Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public IConfiguration Configuration { get; set; }

    public async Task<int> RunAsync()
    {
        _logger.LogInformation("Main executed");

        // use this token for stopping the services
        var cancellationToken = _applicationLifetime.ApplicationStopping;

        var originAddress = new Address(
            streetLine: "11407 Granite Street",
            city: "Charlotte",
            stateOrProvince: "NC",
            postalCode: "28273",
            countryCode: "US");

        var sender = new ContactInfo
        {
            FirstName = "EasyKeys.com",
            LastName = "Customer Support",
            Company = "EasyKeys.com",
            PhoneNumber = "8778395397",
            Email = string.Empty
        };

        // load shipping info for dometic and international
        var models = new List<RateModelDto>();

        var internationalFileName = "Embeded.intnl-addresses.json";
        var internationModels = LoadModels<List<RateModelDto>>(internationalFileName);
        models.AddRange(internationModels);

        var dometsticFileName = "Embeded.domestic-addresses.json";
        var dometicModels = LoadModels<List<RateModelDto>>(dometsticFileName);
        models.AddRange(dometicModels);

        foreach (var model in models)
        {
            // 1. Address validation
            var proposedAddress = await ValidateAsync(model.Address, false, cancellationToken);

            // 2. get rates based on the package size
            var shipments = await GetRatesAsync(originAddress, proposedAddress.ProposedAddress!, model.Packages[0], cancellationToken);
            var rates = shipments.SelectMany(x => x.Rates);
            var flatRates = rates.Select(x => $"{x.Name}:{x.ServiceName}:{x.PackageType} - {x.TotalCharges} - {x.TotalCharges2}");

            foreach (var flatRate in flatRates)
            {
                _logger.LogInformation("{address} - {rate}", model.Address.ToString(), flatRate);
            }

            // if cod = null then nothing to process.
            var cod = new FedExCollectOnDelivery
            {
                Amount = 250,
                CollectionType = FedExCollectionType.GuaranteedFunds,
                Currency = "USD"
            };

            var details = new ShipmentDetails
            {
                Sender = sender,
                Recipient = model.Contact,

                TransactionId = "1234-transaction",

                PaymentType = FedExPaymentType.Sender,

                RateRequestType = "list",

                // CollectOnDelivery = cod,
                // AU with economy is not working
                // DeliverySignatureOptions = "NoSignatureRequired",
                LabelOptions = new LabelOptions()
                {
                    LabelFormatType = "COMMON2D",
                    ImageType = "PNG",
                }
            };

            FedExServiceType? selectedRate = null;
            Shipping.Abstractions.Models.Shipment? shipment = null;

            if (!rates.Any(x => x.Name.Contains("INTERNATIONAL")))
            {
                selectedRate = FedExServiceType.FedExStandardOvernight;
                shipment = GetShipment(shipments, selectedRate);
            }
            else
            {
                selectedRate = FedExServiceType.FedExInternationalPriority;
                shipment = GetShipment(shipments, selectedRate);

                if (model.Commodity != null)
                {
                    details.Commodities.Add(model.Commodity);
                }

                // var shipmentOptions = new ShipmentOptions(FedExPackageType.YourPackaging.Name, DateTime.Now, false);
                // shipment = new Shipping.Abstractions.Models.Shipment(originAddress, proposedAddress.ProposedAddress, model.Packages, shipmentOptions);
            }

            // 3. get shipment label
            var result = await _fedExShipmentProvider.CreateShipmentAsync(selectedRate, shipment!, details, cancellationToken);

            if (result.InternalErrors.Count > 0)
            {
                _logger.LogWarning("Failed {city} to generate label", proposedAddress?.ProposedAddress?.City);
                continue;
            }

            var label = result.Labels[0];

            _logger.LogInformation(
                "{trackingId} - {TotalCharges} - {TotalCharges2}",
                label.TrackingId,
                label.TotalCharges.NetCharge,
                label.TotalCharges2.NetCharge);

            var fileName = $"{model.Address.City}-{label.TrackingId}label.png";

            Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "Labels"));

            await File.WriteAllBytesAsync(Path.Combine(AppContext.BaseDirectory, "Labels", fileName), label.Bytes[0]!);

            var info = await _fedExTrackingProvider.TrackShipmentAsync(label.TrackingId, cancellationToken);
            foreach (var @event in info.TrackingEvents)
            {
                _logger.LogInformation("{eve}", @event.Event);
            }
        }

        return 0;
    }

    private static Shipping.Abstractions.Models.Shipment? GetShipment(
        IEnumerable<Shipping.Abstractions.Models.Shipment> shipments,
        FedExServiceType serviceType)
    {
        foreach (var item in shipments)
        {
            var rate = item?.Rates?.FirstOrDefault(x => x.Name == serviceType.Name);

            if (rate is not null)
            {
                item!.Rates.Clear();
                item.Rates.Add(rate);
                return item;
            }
        }

        return null;
    }

    private static T LoadModels<T>(string fileName) where T : class
    {
        using var stream = EmbeddedResource.GetAsStreamFromCallingAssembly(fileName);
        var models = JsonSerializer.Deserialize(stream!, typeof(T), new JsonSerializerOptions { ReadCommentHandling = JsonCommentHandling.Skip }) as T;

        return models!;
    }

    private async Task<ValidateAddress> ValidateAsync(Address destination, bool debug = false, CancellationToken cancellationToken = default)
    {
        // 1. address validation
        // for international orders, user must enter the provice/state code, not full name
        var request = new ValidateAddress(Guid.NewGuid().ToString(), destination);
        var result = await _validationClient.ValidateAddressAsync(request, cancellationToken);
        var jsonOptions = new JsonSerializerOptions()
        {
            WriteIndented = true
        };

        var originalAddress = JsonSerializer.Serialize(request.OriginalAddress, jsonOptions);
        _logger.LogInformation(Environment.NewLine + "Original Address: {originalAddress}", originalAddress);

        var propsedAddress = JsonSerializer.Serialize(result.ProposedAddress, new JsonSerializerOptions() { WriteIndented = true });
        _logger.LogInformation(Environment.NewLine + "Proposed Address {proposedAddress}", propsedAddress);

        if (debug)
        {
            // 1.a display validation bag
            _logger.LogInformation(Environment.NewLine + result.ValidationBag.Select(x => $"{x.Key}-{x.Value}").Flatten(Environment.NewLine));

            if (result.Errors.Count > 0)
            {
                _logger.LogError($"Address Validation Errors: {result.Errors.Select(x => x.Description).Flatten(Environment.NewLine)}");
            }
        }

        return result;
    }

    private async Task<IEnumerable<Shipping.Abstractions.Models.Shipment>> GetRatesAsync(
        Address origin,
        Address destination,
        Package defaultPackage,
        CancellationToken cancellationToken)
    {
        var shipments = new List<Shipping.Abstractions.Models.Shipment>();

        var config = new FedExRateConfigurator(origin, destination, defaultPackage);

        _logger.LogInformation("Rates for: {address}", destination);

        foreach (var (shipment, serviceType) in config.Shipments)
        {
            // 2. shipment rates
            var result = await _fedexRateProvider.GetRatesAsync(shipment, serviceType, cancellationToken);
            if (result.Errors.Any())
            {
                _logger.LogError("Rates Validation Errors : {errors}", result.Errors.Select(x => x.Description).Flatten(Environment.NewLine));
            }

            foreach (var rate in result.Rates)
            {
                _logger.LogInformation("{serviceName} - {name} - {specialPrice} - {listPrice}", rate.ServiceName, rate.Name, rate.TotalCharges, rate.TotalCharges2);
            }

            if (result != null)
            {
                shipments.Add(result);
            }
        }

        return shipments;
    }
}
