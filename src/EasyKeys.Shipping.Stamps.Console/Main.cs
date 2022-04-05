using System.ServiceModel;

using EasyKeys.Shipping.Stamps.Abstractions.Options;

using Microsoft.Extensions.Options;

using StampsClient.v111;

public class Main : IMain
{
    private readonly ILogger<Main> _logger;
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly StampsOptions _options;

    public Main(
        IOptions<StampsOptions> options,
        IHostApplicationLifetime applicationLifetime,
        IConfiguration configuration,
        ILogger<Main> logger)
    {
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

        var client = new SwsimV111SoapClient(
             new BasicHttpsBinding(BasicHttpsSecurityMode.Transport)
             {
                 MaxReceivedMessageSize = int.MaxValue,
             },
             new EndpointAddress(_options.Url));

        var credentials = new Credentials()
        {
            IntegrationID = new Guid(_options.IntegrationId),
            Username = _options.UserName,
            Password = _options.Password
        };

        var authRequest = new AuthenticateUserRequest(credentials);
        var authResponse = await client.AuthenticateUserAsync(authRequest);
        var authToken = authResponse.Authenticator;

        var rateRequest = new RateV40()
        {
            //PackageType = PackageTypeV11.Letter,
            ShipDate = DateTime.Today.AddDays(5),
            From = new Address()
            {
                State = "NC",
                ZIPCode = "28273",
            },
            To = new Address()
            {
                State = "CA",
                ZIPCode = "90245"
            },
            WeightLb = 0.0,
            WeightOz = 0.25
        };

        var getRatesRequest = new GetRatesRequest(authToken, rateRequest, Carrier.USPS);
        var rateResponse = await client.GetRatesAsync(getRatesRequest);

        //var rateResponse = await client.GetRatesAsync(
        //    new GetRatesRequest
        //    {
        //        Item = credentials,
        //        Rate = rateRequest,
        //        Carrier = Carrier.All
        //    });

        foreach (var rate in rateResponse.Rates)
        {
            var addons = rate.AddOns.Select(x => x.AddOnDescription).Flatten(",");
            _logger.LogInformation($"{rate.ServiceType} - {addons} - {rate.ServiceDescription} - {rate.Amount}");
        }

        return 0;
    }
}
