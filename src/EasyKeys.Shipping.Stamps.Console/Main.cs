using System.Text.Json;

using Bet.Extensions;

using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Options;
using EasyKeys.Shipping.Stamps.AddressValidation;
using EasyKeys.Shipping.Stamps.Rates;
using EasyKeys.Shipping.Stamps.Rates.Models;
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

        var origin = new Address(
            streetLine: "11407 Granite Street",
            city: "Charlotte",
            stateOrProvince: "NC",
            postalCode: "28273",
            countryCode: "US");

        var sender = new ContactInfo()
        {
            FirstName = "EasyKeys.com",
            LastName = "Inc.",
            Company = "EasyKeys.com",
            Email = "info@EasyKeys.com",
            PhoneNumber = "877.839.5397"
        };

        var models = new List<RateModelDto>();

        // var internationalFileName = "Embeded.intnl-addresses.json";
        // var internationModels = LoadModels<List<RateModelDto>>(internationalFileName);
        // models.AddRange(internationModels);
        var dometsticFileName = "Embeded.domestic-addresses.json";
        var dometicModels = LoadModels<List<RateModelDto>>(dometsticFileName);
        if (dometicModels != null)
        {
            models.AddRange(dometicModels);
        }

        await RunConcurrentlyAsync(origin, sender, models, cancellationToken);

        // test tasks based execution
        // await RunNoneConcurrentlyAsync(originAddress, sender, models, cancellationToken);
        return 0;
    }

    private static T? LoadModels<T>(string fileName) where T : class
    {
        using var stream = EmbeddedResource.GetAsStreamFromCallingAssembly(fileName);
        var models = JsonSerializer.Deserialize(stream!, typeof(T), new JsonSerializerOptions { ReadCommentHandling = JsonCommentHandling.Skip }) as T;

        return models;
    }

    private async Task RunNoneConcurrentlyAsync(
        Address originAddress,
        ContactInfo sender,
        List<RateModelDto> models,
        CancellationToken cancellationToken)
    {
        var tasks = new List<Task>();
        var addressValidationTasks = new List<Task<(Address address, bool isValid)>>();
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

        Display(addressValidationTasks, ratesTasks, shipmentTasks);
    }

    private void Display(
        List<Task<(Address addres, bool isValid)>> addressValidationTasks,
        List<Task<IEnumerable<Shipment>>> ratesTasks,
        List<Task<ShipmentLabel>> shipmentTasks)
    {
        // 1. display address validation
        foreach (var validTask in addressValidationTasks)
        {
            var address = validTask.Result;
            _logger.LogInformation("{street} {city} {newLine}", address.addres.StreetLine, address.addres.City, Environment.NewLine);
        }

        // 2. display shipmment rates
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

        // 3. shipment labels
        foreach (var shipmentTask in shipmentTasks)
        {
            var shipmentLabel = shipmentTask.Result;
        }
    }

    private async Task RunConcurrentlyAsync(
        Address origin,
        ContactInfo sender,
        List<RateModelDto> models,
        CancellationToken cancellationToken)
    {
        foreach (var model in models)
        {
            var uuid = Guid.NewGuid().ToString();

            // 1. validate an address
            var validatedRequest = new ValidateAddress(uuid, origin);
            var (address, valid) = await ValidateAsync(model.Address, true, cancellationToken);

            // 2. get rates based on the package dimensions
            var shipments = new List<Shipment>();

            foreach (var package in model.Packages)
            {
                var rate = await GetRatesAsync(origin, address, package, sender, model.Contact, model.Commodity, cancellationToken);
                shipments.AddRange(rate);
            }

            var rates = shipments.SelectMany(x => x.Rates);
            var flatRates = rates.Select(x => $"{x.Name}:{x.ServiceName} - {x.PackageType} - {x.TotalCharges}");

            foreach (var flatRate in flatRates)
            {
                _logger.LogInformation("{address} - {rate}", address.ToString(), flatRate);
            }

            var shipmentDetails = new ShipmentDetails
            {
                IsSample = false
            };

            ShipmentLabel? result;

            // 3. select a quoted rate
            if (address.IsUnitedStatesAddress())
            {
                // USPS First-Class Mail:Package:3.7200
                var selectedRate = rates.SingleOrDefault(x => x.Name == "USPM" && x.PackageType == "SmallFlatRateBox");

                var shipmentOptions = new ShipmentOptions(StampsPackageType.FromName(selectedRate!.PackageType).Name, DateTime.Now);

                var shipment = new Shipment(origin, address, model.Packages, shipmentOptions);

                var rateOptions = new RateOptions
                {
                    Sender = sender,
                    Recipient = model.Contact,
                    ServiceType = StampsServiceType.FromName(selectedRate.Name)
                };

                result = await _shipmentProvider.CreateShipmentAsync(shipment, rateOptions, shipmentDetails, cancellationToken);
            }
            else
            {
                // USPS First-Class Mail International:Package:14.11
                var selectedRate = rates.SingleOrDefault(x => x.Name == "USFCI" && x.PackageType == "Package");

                var shipmentOptions = new ShipmentOptions(StampsPackageType.FromName(selectedRate!.PackageType).Name, DateTime.Now);

                var shipment = new Shipment(origin, address, model.Packages, shipmentOptions);

                shipmentDetails.CustomsInformation.CustomsSigner = "brandon moffett";
                shipmentDetails.CustomsInformation.InvoiceNumber = "12322432";
                shipmentDetails.Commodities.Add(model.Commodity!);

                var rateOptions = new RateOptions
                {
                    Sender = sender,
                    Recipient = model.Contact,
                    ServiceType = StampsServiceType.FromName(selectedRate.Name)
                };

                result = await _shipmentProvider.CreateShipmentAsync(
                    shipment,
                    rateOptions,
                    shipmentDetails,
                    cancellationToken);
            }

            if (result?.InternalErrors.Count > 0)
            {
                _logger.LogWarning("Failed {city} to generate label", address.City);
                continue;
            }

            var label = result?.Labels[0];

            _logger.LogInformation(
                "{trackingId} - {TotalCharges} - {TotalCharges2}",
                label!.TrackingId,
                label.TotalCharges.NetCharge,
                label.TotalCharges2.NetCharge);

            var fileName = $"{model.Address.City}-{label.TrackingId}label.png";

            Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "Labels"));

            await File.WriteAllBytesAsync(Path.Combine(AppContext.BaseDirectory, "Labels", fileName), label.Bytes[0]);

            var trackingInfo = await _trackingProvider.TrackShipmentAsync(label.TrackingId, cancellationToken);

            var cancelReponse = await _shipmentProvider.CancelShipmentAsync(label.TrackingId, cancellationToken);
        }
    }

    private async Task<(Address address, bool valid)> ValidateAsync(Address destination, bool debug = false, CancellationToken cancellationToken = default)
    {
        // 1. address validation
        // for international orders, user must enter the provice/state code, not full name
        var request = new ValidateAddress(Guid.NewGuid().ToString(), destination);
        var result = await _addressProvider.ValidateAddressAsync(request, cancellationToken);

        // no result return intial address
        if (result == null)
        {
            return (destination, false);
        }

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
            _logger.LogInformation(Environment.NewLine + result.ValidationBag.Select(x => $"[{x.Key}]:[{x.Value}]").Flatten(Environment.NewLine));

            if (result.Errors.Count > 0)
            {
                _logger.LogError($"Address Validation Errors: {result.Errors.Select(x => x.Description).Flatten(Environment.NewLine)}");
            }
        }

        var isValidated = false;

        // international valid addresses
        if (!destination.IsUnitedStatesAddress()
            && result.Errors.Count == 0
            && result.ValidationBag["CityStateZipOK"] == "True"
            && result.ValidationBag["AddressMatch"] == "True"
            && result.ValidationBag["ValidationResult"] == "To Country Verified.")
        {
            isValidated = true;
        }

        // domestic valid addresses
        if (!destination.IsUnitedStatesAddress()
            && result.Errors.Count == 0
            && result.ValidationBag["CityStateZipOK"] == "True"
            && result.ValidationBag["AddressMatch"] == "True"
            && result.ValidationBag["ValidationResult"] == "Full Address Verified.")
        {
            isValidated = true;
        }

        return (result.ProposedAddress!, isValidated);
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
        // 1. configurator selects the correct pacakge type based on the dimensions
        var shipmentConfigurator = new StampsRateConfigurator(origin, destination, package);

        var shipments = new List<Shipment>();

        // 2. now shipments contains different packages to be used to query the rate service.
        foreach (var shipment in shipmentConfigurator.Shipments)
        {
            Shipment? result = null;

            if (destination.IsUnitedStatesAddress())
            {
                // 2.1 add commonidity for the international mail
                var rateOptions = new RateOptions
                {
                    Sender = sender,
                    Recipient = receiver,
                };

                // 2.2. call the services for specified mail.
                result = await _rateProvider.GetRatesAsync(shipment, rateOptions, cancellationToken);
            }
            else
            {
                var rateOptions = new RateOptions
                {
                    Sender = sender,
                    Recipient = receiver,
                };

                result = await _rateProvider.GetRatesAsync(shipment, rateOptions, cancellationToken);
            }

            if (result.Errors.Count > 0)
            {
                _logger.LogError("Rates Validation Errors : {errors}", result.Errors.Select(x => x.Description).Flatten(Environment.NewLine));
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
        var config = new StampsRateConfigurator(origin, destination, package);

        var rateOptions = new RateOptions
        {
            Sender = sender,
            Recipient = receiver,
        };

        var shipmentDetails = new ShipmentDetails();

        return await _shipmentProvider.CreateShipmentAsync(config.Shipments[0], rateOptions, shipmentDetails, cancellationToken);
    }
}
