using System.Text.Json;

using Bet.Extensions;

using EasyKeys.Shipping.Abstractions.Models;
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

        var sender = new ContactInfo()
        {
            FirstName = "EasyKeys.com",
            LastName = string.Empty,
            Company = "EasyKeys.com",
            Email = "info@EasyKeys.com",
            PhoneNumber = "877.839.5397"
        };

        var models = new List<RateModelDto>();

        var internationalFileName = "Embeded.intnl-addresses.json";
        var internationModels = LoadModels<List<RateModelDto>>(internationalFileName);
        models.AddRange(internationModels);

        var dometsticFileName = "Embeded.domestic-addresses.json";
        var dometicModels = LoadModels<List<RateModelDto>>(dometsticFileName);
        models.AddRange(dometicModels);

        var startTime = DateTime.Now;

        var tasks = new List<Task>();
        var addressValidationTasks = new List<Task<Address>>();
        var ratesTasks = new List<Task<IEnumerable<Shipment>>>();
        var shipmentTasks = new List<Task<ShipmentLabel>>();

        foreach (var model in models)
        {
            var addressValidationTask = ValidateAsync(model.Address, true, cancellationToken);
            addressValidationTasks.Add(addressValidationTask);

            var rateTask = GetRatesAsync(originAddress, model.Address, model.Packages[0], sender, model.Contact, model.Commodity, cancellationToken);
            ratesTasks.Add(rateTask);

            var shipmentTask = CreateShipmentAsync(originAddress, model.Address, model.Packages[0], sender, model.Contact, cancellationToken);
            shipmentTasks.Add(shipmentTask);
        }

        tasks.AddRange(addressValidationTasks);
        tasks.AddRange(ratesTasks);
        tasks.AddRange(shipmentTasks);

        await Task.WhenAll(tasks.ToArray());

        // 1. display address validation
        foreach (var validTask in addressValidationTasks)
        {
            var address = validTask.Result;
            _logger.LogInformation("{street} {city} {newLine}", address.StreetLine, address.City, Environment.NewLine);
        }

        // display shipmment rates
        foreach (var rateTask in ratesTasks)
        {
            var rateShipments = rateTask.Result;
            _logger.LogInformation(
                    "{street} {city}",
                    rateShipments.First().DestinationAddress.StreetLine,
                    rateShipments.First().DestinationAddress.City);

            foreach (var rateShipment in rateShipments)
            {
                foreach (var rate in rateShipment.Rates)
                {
                    _logger.LogInformation("{name}-{packageType}-{serviceName} => {charge}", rate.Name, rate.PackageType, rate.ServiceName, rate.TotalCharges);
                }
            }

            _logger.LogInformation(Environment.NewLine);
        }

        return 0;

        // 3) create shipment
        //var config = new StampsRateConfigurator(
        //    originAddress,
        //    validatedAddress.ProposedAddress ?? validatedAddress.OriginalAddress,
        //    packages.First(),
        //    sender,
        //    receiver);

        //var (shipment, ratesOptions) = config.Shipments.First();

        //shipment.Errors.Concat(validatedAddress.Errors);

        //if (!destinationAddress.IsUnitedStatesAddress())
        //{
        //    shipment.Commodities.Add(commodity);
        //}

        //// 4) create generic rate details
        //var rateDetails = new RateRequestDetails();

        //// 5) get list of rates for shipment
        //var shipmentWithRates = await _rateProvider.GetRatesAsync(config.Shipments.FirstOrDefault().shipment, rateDetails, cancellationToken);

        //_logger.LogError($"Rates Validation Errors : {shipmentWithRates.Errors.Count()}");

        //// user chooses which type of service
        //var shipmentDetails = new ShipmentRequestDetails() { DeclaredValue = 100m, SelectedRate = shipmentWithRates.Rates[0] };

        //if (!destinationAddress.IsUnitedStatesAddress())
        //{
        //    shipmentDetails.CustomsInformation = new CustomsInformation() { CustomsSigner = "brandon moffett" };
        //}

        //// 6) create shipment with shipment details
        //var shipmentResponse = await _shipmentProvider.CreateShipmentAsync(shipmentWithRates, shipmentDetails, cancellationToken);

        //_logger.LogCritical($"Tracking Number : {shipmentResponse.Labels[0].TrackingId}");

        //await File.WriteAllBytesAsync("label.png", shipmentResponse.Labels[0].Bytes[0]);

        //var trackingInfo = await _trackingProvider.TrackShipmentAsync(shipmentResponse.Labels[0].TrackingId, cancellationToken);

        //var cancelReponse = await _shipmentProvider.CancelShipmentAsync(shipmentResponse.Labels[0].TrackingId, cancellationToken);

        //return 0;
    }

    private static T? LoadModels<T>(string fileName) where T : class
    {
        using var stream = EmbeddedResource.GetAsStreamFromCallingAssembly(fileName);
        var models = JsonSerializer.Deserialize(stream, typeof(T), new JsonSerializerOptions { ReadCommentHandling = JsonCommentHandling.Skip }) as T;

        return models;
    }

    private async Task<Address> ValidateAsync(Address destination, bool debug = false, CancellationToken cancellationToken = default)
    {
        // 1. address validation
        // for international orders, user must enter the provice/state code, not full name
        var request = new ValidateAddress(Guid.NewGuid().ToString(), destination);
        var result = await _addressProvider.ValidateAddressAsync(request, cancellationToken);

        // no result return intial address
        if (result == null)
        {
            return destination;
        }

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

            _logger.LogWarning($"Address Validation Result : {result.ValidationBag.Select(x => x.Key + ": " + x.Value).Aggregate((x, y) => x + ", " + y)}");

            if (result.Errors.Count > 0)
            {
                _logger.LogError($"Address Validation Errors: {result.Errors.Select(x => x.Description).Flatten(Environment.NewLine)}");
            }
        }

        return result.ProposedAddress ?? result.OriginalAddress;
    }

    private async Task<IEnumerable<Shipment>> GetRatesAsync(
        Address origin,
        Address destination,
        Package package,
        ContactInfo sender,
        ContactInfo receiver,
        Commodity? commodity = null,
        CancellationToken cancellationToken = default)
    {
        var shipmentConfigurator = new StampsRateConfigurator(origin, destination, package, sender, receiver);

        var shipments = new List<Shipment>();

        foreach (var shipment in shipmentConfigurator.Shipments)
        {
            if (!destination.IsUnitedStatesAddress()
                && commodity != null)
            {
                shipment.shipment.Commodities.Add(commodity);
            }

            var result = await _rateProvider.GetRatesAsync(shipment.shipment, shipment.rateOptions, cancellationToken);

            if (result.Errors.Count > 0)
            {
                _logger.LogError($"Rates Validation Errors : {result.Errors.Select(x => x.Description).Flatten(Environment.NewLine)}");
            }

            if (result != null)
            {
                shipments.Add(result);
            }
        }

        return shipments;
    }

    private async Task<ShipmentLabel> CreateShipmentAsync(
        Address origin,
        Address destination,
        Package package,
        ContactInfo sender,
        ContactInfo receiver,
        CancellationToken cancellationToken = default)
    {
        var config = new StampsRateConfigurator(origin, destination, package, sender, receiver);

        var shipmentDetails = new ShipmentRequestDetails
        {
            RateRequestDetails = config.Shipments[0].rateOptions
        };

        return await _shipmentProvider.CreateShipmentAsync(config.Shipments[0].shipment, shipmentDetails, cancellationToken);
    }
}
