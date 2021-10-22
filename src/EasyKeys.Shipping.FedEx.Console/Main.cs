using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.FedEx.AddressValidation;

namespace EasyKeys.Shipping.FedEx.Console;

public class Main : IMain
{
    private readonly IFedExAddressValidationProvider _validationClient;
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly ILogger<Main> _logger;

    public Main(
        IFedExAddressValidationProvider validationClient,
        IHostApplicationLifetime applicationLifetime,
        IConfiguration configuration,
        ILogger<Main> logger)
    {
        _validationClient = validationClient ?? throw new ArgumentNullException(nameof(validationClient));
        _applicationLifetime = applicationLifetime ?? throw new ArgumentNullException(nameof(applicationLifetime));
        Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public IConfiguration Configuration { get; set; }

    public async Task<int> RunAsync()
    {
        _logger.LogInformation("Main executed");

        // use this token for stopping the services
        _applicationLifetime.ApplicationStopping.ThrowIfCancellationRequested();

        // var address2 = new ValidateAddress(
        //                Guid.NewGuid().ToString(),
        //                new Shipping.Abstractions.Address(
        //                "One Microsoft Way",
        //                "",
        //                "Redmond",
        //                "Washington",
        //                "98052",
        //                "US",
        //                false));

        // var address2 = new ValidateAddress(
        //            Guid.NewGuid().ToString(),
        //            new Shipping.Abstractions.Address(
        //            "Mauerberger  Building",
        //            "2nd floor",
        //            "Technion City",
        //            "Haifa",
        //            "3200003",
        //            "IL",
        //            false));

        // var address2 = new ValidateAddress(
        //        Guid.NewGuid().ToString(),
        //        new Shipping.Abstractions.Address(
        //        "100 East Capitol Street",
        //        "Suite 1000",
        //        "Jackson",
        //        "MS",
        //        "39201",
        //        "US",
        //        false));
        var address2 = new ValidateAddress(
            Guid.NewGuid().ToString(),
            new Shipping.Abstractions.Address(
            "1500 S STRONG DR",
            "Apartment, suite, unit, etc. (optional)",
            "BLOOMINGTON",
            "IL",
            "47403-8741",
            "US",
            false));

        var result2 = await _validationClient.ValidateAddressAsync(address2);
        result2.ValidationBag.TryGetValue("State", out var v);
        _logger.LogInformation("{isVerified}", v);

        return await Task.FromResult(0);
    }
}
