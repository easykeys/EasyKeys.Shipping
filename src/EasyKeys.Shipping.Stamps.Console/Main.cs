
using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Options;
using EasyKeys.Shipping.Stamps.AddressValidation;
using EasyKeys.Shipping.Stamps.Rates;
using EasyKeys.Shipping.Stamps.Shipment;

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
            streetLine: "1550 Central Ave",
            city: "Riverside",
            stateOrProvince: "CA",
            postalCode: "92507",
            countryCode: "US");

        var packages = new List<EasyKeys.Shipping.Abstractions.Package>
        {
            new EasyKeys.Shipping.Abstractions.Package(
                new EasyKeys.Shipping.Abstractions.Dimensions()
                {
                    Height = 20.00M,
                    Width = 20.00M,
                    Length = 20.00M
                },
                1.0M),
        };

        var sender = new SenderContact()
        {
            FullName = "Brandon Moffett",
            CompanyName = "EasyKeys.com",
            Email = "TestMe@EasyKeys.com",
            Department = "Software"
        };
        var receiver = new RecipientContact()
        {
            FullName = "Fictitious Character",
            CompanyName = "Marvel",
            Email = "FictitiousCharacter@marvel.com",
            Department = "SuperHero"
        };

        var validateRequest = new ValidateAddress(Guid.NewGuid().ToString(), destinationAddress);

        var validateResponse = await _addressProvider.ValidateAddressAsync(validateRequest, cancellationToken);

        _logger.LogInformation($"Address Validation Errors: {validateResponse.Errors?.FirstOrDefault()?.Description}");

        var shipment = new Shipment(originAddress, validateResponse.OriginalAddress, packages) { RecipientContact = receiver, SenderContact = sender };

        _logger.LogInformation($"Errors: {shipment.Errors.Count}");

        var rateResponse = await _rateProvider.GetRatesAsync(shipment, new ShipmentDetails(), default, cancellationToken);

        var shipmentResponse = await _shipmentProvider.CreateShipmentAsync(
            shipment,
            new ShipmentDetails() { Carrier = "USPS", ServiceDescription = "USPS Priority Mail", Sender = sender, Recipient = receiver },
            EasyKeys.Shipping.Stamps.Abstractions.Models.ServiceType.USPS_PRIORITY_MAIL,
            cancellationToken);

        _logger.LogCritical($"Tracking Number : {shipmentResponse.Labels[0].TrackingId}");

        await File.WriteAllBytesAsync("label.png", shipmentResponse.Labels[0].Bytes[0]);

        var cancelReponse = await _shipmentProvider.CancelShipmentAsync(shipmentResponse, cancellationToken);

        return 0;
    }
}
