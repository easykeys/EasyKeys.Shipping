using Bet.Extensions.Testing.Logging;

using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.AddressValidation;
using EasyKeys.Shipping.Stamps.AddressValidation.DependencyInjection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EasyKeysShipping.UnitTest
{
    public class StampsAddressValidationProviderTests
    {
        private readonly ITestOutputHelper _output;
        private readonly IStampsAddressValidationProvider _validator;

        public StampsAddressValidationProviderTests(ITestOutputHelper output)
        {
            _output = output;
            _validator = GetAddressValidator();
        }

        [Fact]
        public async Task Domestic_Address_Validation_Successfully()
        {
            var cancellationToken = CancellationToken.None;
            var request = new ValidateAddress(
                Guid.NewGuid().ToString(),
                new EasyKeys.Shipping.Abstractions.Models.Address(
                   "ATTN John Smith 1800 ISLE PKWY",
                   string.Empty,
                   "BETTENDORF",
                   "IA",
                   "52722",
                   "US",
                   false));

            var result = await _validator.ValidateAddressAsync(request, cancellationToken);
        }

        [Fact]
        public async Task International_Address_Validation_Successfully()
        {
            var cancellationToken = CancellationToken.None;
            var request = new ValidateAddress(
                Guid.NewGuid().ToString(),
                new EasyKeys.Shipping.Abstractions.Models.Address(
                   "ATTN John Smith 1800 ISLE PKWY",
                   string.Empty,
                   "BETTENDORF",
                   "IA",
                   "52722",
                   "US",
                   false));

            var result = await _validator.ValidateAddressAsync(request, cancellationToken);
        }

        [Fact]
        public async Task Error_Returned_Successfully()
        {
            var cancellationToken = CancellationToken.None;
            var request = new ValidateAddress(
                Guid.NewGuid().ToString(),
                new EasyKeys.Shipping.Abstractions.Models.Address(
                   "ATTN John Smith 1800 ISLE PKWY",
                   string.Empty,
                   "BETTENDORF",
                   "IA",
                   "52722",
                   "US",
                   false));

            var result = await _validator.ValidateAddressAsync(request, cancellationToken);
        }

        [Fact]

        public async Task Warning_Returned_Successfully()
        {
            var cancellationToken = CancellationToken.None;
            var request = new ValidateAddress(
                Guid.NewGuid().ToString(),
                new EasyKeys.Shipping.Abstractions.Models.Address(
                   "ATTN John Smith 1800 ISLE PKWY",
                   string.Empty,
                   "BETTENDORF",
                   "IA",
                   "52722",
                   "US",
                   false));

            var result = await _validator.ValidateAddressAsync(request, cancellationToken);
        }

        private IStampsAddressValidationProvider GetAddressValidator()
        {
            var services = new ServiceCollection();

            var dic = new Dictionary<string, string>
            {
                { "AzureVault:BaseUrl", "https://easykeys.vault.azure.net/" },
            };

            var configBuilder = new ConfigurationBuilder().AddInMemoryCollection(dic);
            configBuilder.AddAzureKeyVault(hostingEnviromentName: "Development", usePrefix: true);

            services.AddLogging(builder => builder.AddXunit(_output));
            services.AddSingleton<IConfiguration>(configBuilder.Build());

            services.AddStampsClient();
            services.AddStampsAddressProvider();

            var sp = services.BuildServiceProvider();
            return sp.GetRequiredService<IStampsAddressValidationProvider>();
        }
    }
}
