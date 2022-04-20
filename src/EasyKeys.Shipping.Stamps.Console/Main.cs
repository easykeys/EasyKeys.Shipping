
using System.Net;

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
                50.0M),
        };
        var shipment = new Shipment(originAddress, destinationAddress, packages);

        var addressResponse = await _addressProvider.ValidateAddressAsync(shipment, cancellationToken);

        _logger.LogInformation($"{shipment.DestinationAddress.IsResidential}");

        var rateResponse = await _rateProvider.GetRatesAsync(shipment, cancellationToken);

        var shipmentResponse = await _shipmentProvider.CreateShipmentAsync(shipment, rateResponse?.Rates?.LastOrDefault(), cancellationToken);

        _logger.LogCritical($"Label Url: {shipmentResponse.URL}");

        var url = $"{shipmentResponse.URL}";

        using (var client = new WebClient())
        {
            client.DownloadFile(new Uri(url), "Label.png");
        }

        return 0;
    }
}
