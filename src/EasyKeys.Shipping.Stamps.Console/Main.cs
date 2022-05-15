using System.Text.Json;

using Bet.Extensions;

using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Options;
using EasyKeys.Shipping.Stamps.AddressValidation;
using EasyKeys.Shipping.Stamps.Rates;
using EasyKeys.Shipping.Stamps.Shipment;
using EasyKeys.Shipping.Stamps.Shipment.Models;
using EasyKeys.Shipping.Stamps.Tracking;

using Microsoft.Extensions.Options;

using Models;

public class Main : IMain
{
    private readonly ILogger<Main> _logger;
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly IStampsAddressValidationProvider _addressProvider;
    private readonly IStampsRateProvider _rateProvider;
    private readonly IStampsShipmentProvider _shipmentProvider;
    private readonly IStampsTrackingProvider _trackingProvider;
    private readonly StampsOptions _options;

    public Main(
        IOptions<StampsOptions> options,
        IHostApplicationLifetime applicationLifetime,
        IConfiguration configuration,
        IStampsRateProvider rateProvider,
        IStampsShipmentProvider shipmentProvider,
        IStampsAddressValidationProvider addressProvider,
        IStampsTrackingProvider trackingProvider,
        ILogger<Main> logger)
    {
        _rateProvider = rateProvider;
        _addressProvider = addressProvider;
        _shipmentProvider = shipmentProvider;
        _options = options.Value;
        _applicationLifetime = applicationLifetime ?? throw new ArgumentNullException(nameof(applicationLifetime));
        Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _trackingProvider = trackingProvider;
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

        var destinationAddress = new Address(
            streetLine: "1550 central ave",
            city: "riverside",
            stateOrProvince: "CA",
            postalCode: "92507",
            countryCode: "US");

        var packages = new List<Package>
        {
            new Package(
                new Dimensions()
                {
                    Height = 20.00M,
                    Width = 20.00M,
                    Length = 20.00M
                },
                50.0M),
        };

        var commodity = new Commodity()
        {
            Description = "ekjs",
            CountryOfManufacturer = "US",
            PartNumber = "kjsdf",
            Amount = 10m,
            CustomsValue = 1m,
            NumberOfPieces = 1,
            Quantity = 1,
            ExportLicenseNumber = "dsdfs",
            Name = "sdkfsdf",
        };

        var sender = new ContactInfo()
        {
            FirstName = "Brandon",
            LastName = "Moffett",
            Company = "EasyKeys.com",
            Email = "TestMe@EasyKeys.com",
            Department = "Software",
            PhoneNumber = "951-223-2222"
        };
        var receiver = new ContactInfo()
        {
            FirstName = "Fictitious Character",
            Company = "Marvel",
            Email = "FictitiousCharacter@marvel.com",
            Department = "SuperHero",
            PhoneNumber = "867-338-2737"
        };

        var fileName = "Embeded.intnl-addresses.json";
        //var fileName = "Embeded.domestic-addresses.json";
        var models = LoadModels<List<RateModelDto>>(fileName);

        foreach (var model in models)
        {
            var proposedAddress = await ValidateAsync(model.Address, true, cancellationToken);
        }

        // 1) create validate address request
        var validateRequest = new ValidateAddress(Guid.NewGuid().ToString(), destinationAddress);

        // 2) validate address
        var validatedAddress = await _addressProvider.ValidateAddressAsync(validateRequest, cancellationToken);

        _logger.LogWarning($"Address Validation Result : {validatedAddress.ValidationBag.Select(x => x.Key + ": " + x.Value).Aggregate((x, y) => x + ", " + y)}");

        _logger.LogError($"Address Validation Errors: {validatedAddress.Errors.Count()}");

        // 3) create shipment
        var config = new StampsRateConfigurator(
            originAddress,
            validatedAddress.ProposedAddress ?? validatedAddress.OriginalAddress,
            packages.First(),
            sender,
            receiver);

        var (shipment, ratesOptions) = config.Shipments.First();

        shipment.Errors.Concat(validatedAddress.Errors);

        if (!destinationAddress.IsUnitedStatesAddress())
        {
            shipment.Commodities.Add(commodity);
        }

        // 4) create generic rate details
        var rateDetails = new RateRequestDetails();

        // 5) get list of rates for shipment
        var shipmentWithRates = await _rateProvider.GetRatesAsync(config.Shipments.Select(x => x.shipment).ToList(), rateDetails, cancellationToken);

        _logger.LogError($"Rates Validation Errors : {shipmentWithRates.Errors.Count()}");

        // user chooses which type of service
        var shipmentDetails = new ShipmentRequestDetails() { DeclaredValue = 100m, SelectedRate = shipmentWithRates.Rates[0] };

        if (!destinationAddress.IsUnitedStatesAddress())
        {
            shipmentDetails.CustomsInformation = new CustomsInformation() { CustomsSigner = "brandon moffett" };
        }

        // 6) create shipment with shipment details
        var shipmentResponse = await _shipmentProvider.CreateShipmentAsync(shipmentWithRates, shipmentDetails, cancellationToken);

        _logger.LogCritical($"Tracking Number : {shipmentResponse.Labels[0].TrackingId}");

        await File.WriteAllBytesAsync("label.png", shipmentResponse.Labels[0].Bytes[0]);

        var trackingInfo = await _trackingProvider.TrackShipmentAsync(shipmentResponse, cancellationToken);

        var cancelReponse = await _shipmentProvider.CancelShipmentAsync(shipmentResponse, cancellationToken);

        return 0;
    }

    private static T LoadModels<T>(string fileName) where T : class
    {
        using var stream = EmbeddedResource.GetAsStreamFromCallingAssembly(fileName);
        var models = JsonSerializer.Deserialize(stream, typeof(T), new JsonSerializerOptions { ReadCommentHandling = JsonCommentHandling.Skip }) as T;

        return models;
    }

    private async Task<ValidateAddress> ValidateAsync(Address destination, bool debug = false, CancellationToken cancellationToken = default)
    {
        // 1. address validation
        // for international orders, user must enter the provice/state code, not full name


        var request = new ValidateAddress(Guid.NewGuid().ToString(), destination);
        var result = await _addressProvider.ValidateAddressAsync(request, cancellationToken);

        // var originalAddress = JsonSerializer.Serialize(validationResult.OriginalAddress, new JsonSerializerOptions() { WriteIndented = true });
        // _logger.LogInformation(originalAddress);
        if (request.OriginalAddress != request.ProposedAddress && debug)
        {
            var propsedAddress = JsonSerializer.Serialize(result.ProposedAddress, new JsonSerializerOptions() { WriteIndented = true });
            _logger.LogInformation(propsedAddress);
        }

        if (debug)
        {
            // 1.a display validation bag
            _logger.LogInformation(Environment.NewLine + result.ValidationBag.Select(x => $"{x.Key}-{x.Value}").Flatten(Environment.NewLine));
        }

        return result;
    }
}
