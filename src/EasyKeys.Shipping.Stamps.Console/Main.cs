
using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Options;
using EasyKeys.Shipping.Stamps.AddressValidation;
using EasyKeys.Shipping.Stamps.Rates;
using EasyKeys.Shipping.Stamps.Shipment;
using EasyKeys.Shipping.Stamps.Shipment.Models;

using Microsoft.Extensions.Options;

public class Main : IMain
{
    private readonly ILogger<Main> _logger;
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly IStampsAddressValidationProvider _addressProvider;
    private readonly IStampsRateProvider _rateProvider;
    private readonly IStampsShipmentProvider _shipmentProvider;
    private readonly StampsOptions _options;

    public Main(
        IOptions<StampsOptions> options,
        IHostApplicationLifetime applicationLifetime,
        IConfiguration configuration,
        IStampsRateProvider rateProvider,
        IStampsShipmentProvider shipmentProvider,
        IStampsAddressValidationProvider addressProvider,
        ILogger<Main> logger)
    {
        _rateProvider = rateProvider;
        _addressProvider = addressProvider;
        _shipmentProvider = shipmentProvider;
        _options = options.Value;
        _applicationLifetime = applicationLifetime ?? throw new ArgumentNullException(nameof(applicationLifetime));
        Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public IConfiguration Configuration { get; set; }

    public async Task<int> RunAsync()
    {
        _logger.LogInformation("Main executed");

        // use this token for stopping the services
        var cancellationToken = _applicationLifetime.ApplicationStopping;

        var originAddress = new EasyKeys.Shipping.Abstractions.Models.Address(
            streetLine: "11407 Granite Street",
            city: "Charlotte",
            stateOrProvince: "NC",
            postalCode: "28273",
            countryCode: "US");

        var destinationAddress = new EasyKeys.Shipping.Abstractions.Models.Address(
            streetLine: "24 Sussex Drive",
            city: "Ottawa",
            stateOrProvince: "ON",
            postalCode: "K1M 1M4",
            countryCode: "CA");

        var packages = new List<EasyKeys.Shipping.Abstractions.Package>
        {
            new EasyKeys.Shipping.Abstractions.Package(
                new EasyKeys.Shipping.Abstractions.Dimensions()
                {
                    Height = 20.00M,
                    Width = 20.00M,
                    Length = 20.00M
                },
                50.0M),
        };

        var commodity = new Commodity() { Description = "ekjs", CountryOfManufacturer = "US", PartNumber = "kjsdf", Amount = 10m, CustomsValue = 1m, NumberOfPieces = 1, Quantity = 1, ExportLicenseNumber = "dsdfs", Name = "sdkfsdf", Weight = 13m };

        var sender = new SenderInformation()
        {
            FullName = "Brandon Moffett",
            Company = "EasyKeys.com",
            Email = "TestMe@EasyKeys.com",
            Department = "Software",
            PhoneNumber = "951-223-2222"
        };
        var receiver = new RecipientInformation()
        {
            FullName = "Fictitious Character",
            Company = "Marvel",
            Email = "FictitiousCharacter@marvel.com",
            Department = "SuperHero",
            PhoneNumber = "867-338-2737"
        };

        // 1) create validate address request
        var validateRequest = new ValidateAddress(Guid.NewGuid().ToString(), destinationAddress);

        // 2) validate address
        var validatedAddress = await _addressProvider.ValidateAddressAsync(validateRequest, cancellationToken);

        _logger.LogWarning($"Address Validation Warnings : {validatedAddress.Warnings.Count()}");

        _logger.LogError($"Address Validation Warnings : {validatedAddress.Errors.Count()}");

        // 3) create shipment
        var shipment = new Shipment(originAddress, validatedAddress.ProposedAddress ?? validatedAddress.OriginalAddress, packages)
        {
            RecipientInformation = receiver,
            SenderInformation = sender,
        };

        shipment.Errors.Concat(validatedAddress.Errors);

        shipment.Warnings.Concat(validatedAddress.Warnings);

        shipment.Commodities.Add(commodity);

        // 4) create generic rate details
        var rateDetails = new RateRequestDetails() { DeclaredValue = 100m, RegisteredValue = 100m };

        // 5) get list of rates for shipment
        var shipmentWithRates = await _rateProvider.GetRatesAsync(shipment, rateDetails, cancellationToken);

        _logger.LogWarning($"Address Validation Warnings : {shipmentWithRates.Warnings.Count()}");

        _logger.LogError($"Address Validation Warnings : {shipmentWithRates.Errors.Count()}");

        // user chooses which type of service
        var shipmentDetails = new ShipmentRequestDetails() { DeclaredValue = 100m, SelectedRate = shipmentWithRates.Rates[0], CustomsInformation = new CustomsInformation() { CustomsSigner = "brandon moffett" } };

        // 6) create shipment with shipment details
        var shipmentResponse = await _shipmentProvider.CreateShipmentAsync(shipmentWithRates, shipmentDetails, cancellationToken);

        _logger.LogCritical($"Tracking Number : {shipmentResponse.Labels[0].TrackingId}");

        await File.WriteAllBytesAsync("label.png", shipmentResponse.Labels[0].Bytes[0]);

        var cancelReponse = await _shipmentProvider.CancelShipmentAsync(shipmentResponse, cancellationToken);

        return 0;
    }
}
