using System.Collections;

using Bet.Extensions.Testing.Logging;

using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.AddressValidation;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EasyKeysShipping.UnitTest.Stamps;

public class StampsAddressValidationProviderTests
{
    private readonly ITestOutputHelper _output;
    private readonly IStampsAddressValidationProvider _validator;

    public StampsAddressValidationProviderTests(ITestOutputHelper output)
    {
        _output = output;
        _validator = GetAddressValidator();
    }

    [Theory]
    [ClassData(typeof(AddressTestData))]
    public async Task Address_Validation_Successfully(
        Address address, int errorCount, int internalErrorCount, int warningCount)
    {
        var cancellationToken = CancellationToken.None;

        var request = new ValidateAddress(
            Guid.NewGuid().ToString(),
            address);

        var result = await _validator.ValidateAddressAsync(request, cancellationToken);

        Assert.NotNull(result);

        Assert.Equal(internalErrorCount, result.InternalErrors.Count());

        Assert.Equal(errorCount, result.Errors.Count());

        Assert.Equal(warningCount, result.Warnings.Count());
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
        services.AddStampsAddressProvider();

        var sp = services.BuildServiceProvider();
        return sp.GetRequiredService<IStampsAddressValidationProvider>();
    }

    public class AddressTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                 new Address()
                        {
                            StreetLine = "1550 Central Ave",
                            StreetLine2 = "Apt 35",
                            City = "Riverside",
                            StateOrProvince = "CA",
                            CountryCode = "US",
                            PostalCode = "92507"
                        },

                 // Errors
                 0,

                 // Internal Errors
                 0,

                 // Warnings
                 0
            };
            yield return new object[]
            {
                 new Address()
                        {
                            City = "Riverside",
                            StateOrProvince = "CA",
                            CountryCode = "US",
                            PostalCode = "92507"
                        },

                 // Errors
                 0,

                 // Internal Errors
                 1,

                 // Warnings
                 0
            };
            yield return new object[]
            {
                 new Address()
                        {
                            City = "Riverside",
                            StreetLine = "is this a real street",
                            StateOrProvince = "CA",
                            CountryCode = "US",
                            PostalCode = "92507"
                        },

                 // Errors
                 0,

                 // Internal Errors
                 0,

                 // Warnings
                 1
            };
            yield return new object[]
            {
                 // International Address
                 new Address()
                        {
                            City = "San Diana",
                            StreetLine = "Strada Gilda 2 Piano 9",
                            StateOrProvince = "Brescia",
                            CountryCode = "IT",
                            PostalCode = "64921"
                        },

                 // Errors
                 0,

                 // Internal Errors
                 0,

                 // Warnings
                 0
            };
            yield return new object[]
            {
                 // International Address
                 new Address()
                        {
                            City = "Barrhead",
                            StreetLine = "512 Venture Place",
                            StateOrProvince = "AB",
                            CountryCode = "CA",
                            PostalCode = "T0G 0E0"
                        },

                 // Errors
                 0,

                 // Internal Errors
                 0,

                 // Warnings
                 0
            };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
