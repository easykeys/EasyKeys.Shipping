using System;
using System.Threading.Tasks;

using EasyKeys.Shipping.FedEx.AddressValidation;
using EasyKeys.Shipping.FedEx.AddressValidation.Models;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EasyKeys.Shipping.FedEx.Console
{
    public class Main : IMain
    {
        private readonly IValidationClient _validationClient;
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly ILogger<Main> _logger;

        public Main(
            IValidationClient validationClient,
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

            // var address1 = new ValidationRequest()
            // {
            //    Address = new PostalAddress
            //    {
            //        Address = "One Microsoft Way",
            //        City = "Redmond",
            //        StateOrProvince = "Washington",
            //        PostalCode = "98052-6399",
            //        CountryCode = "US"
            //    }
            // };

            // var result1 = await _validationClient.ValidateAddressAsync(address1);

            // _logger.LogInformation("{score}", result1.Score);
            var address2 = new ValidationRequest()
            {
                Address = new PostalAddress
                {
                    Address = "Mauerberger  Building",
                    Address2 = "2nd floor",
                    City = "Technion City",
                    StateOrProvince = "Haifa",
                    PostalCode = "3200003",
                    CountryCode = "IL"
                }
            };

            var result2 = await _validationClient.ValidateAddressAsync(address2);
            _logger.LogInformation("{score}", result2.Score);

            return await Task.FromResult(0);
        }
    }
}
