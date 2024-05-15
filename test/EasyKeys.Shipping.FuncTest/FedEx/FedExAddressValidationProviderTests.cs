using Bet.Extensions.Testing.Logging;

using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.FedEx.AddressValidation;

using EasyKeysShipping.FuncTest.TestHelpers;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EasyKeysShipping.FuncTest.FedEx;

public class FedExAddressValidationProviderTests
{
    private readonly ITestOutputHelper _output;
    private readonly IEnumerable<IFedExAddressValidationProvider> _validators;

    public FedExAddressValidationProviderTests(ITestOutputHelper output)
    {
        _output = output;
        _validators = ServiceProviderInstance.GetFedExServices(output).GetServices<IFedExAddressValidationProvider>();
    }

    [Fact]
    public async Task NORMALIZED_Unknown_Address_Successfully()
    {
        var request = new ValidateAddress(
            Guid.NewGuid().ToString(),
            new Address(
               "ATTN John Smith 1800 ISLE PKWY",
               string.Empty,
               "BETTENDORF",
               "IA",
               "52722",
               "US",
               false));

        foreach (var validator in _validators)
        {
            var result = await validator.ValidateAddressAsync(request);

            var proposed = result.ProposedAddress;
            Assert.NotNull(proposed);
            Assert.Equal(string.Empty, proposed?.StreetLine);
            Assert.Equal(string.Empty, proposed?.StreetLine2);
            Assert.Equal("52722", proposed?.PostalCode);

            // BUSINESS, RESIDENTIAL
            // MIXED (If it is a multi-tenant based address and contains both business and residential units.)
            // UNKNOWN (If just a zip code is provided, Address Validation Service returns 'unknown' for the business/residential classification)
            Assert.Equal("UNKNOWN", result.ValidationBag.GetValueOrDefault("Classification"));

            // If the address returned includes the address state of "Standardized" and also if the attributes of Resolved = True,
            // DPV = True are present, then the address is likely a valid one.
            Assert.Equal("NORMALIZED", result.ValidationBag.GetValueOrDefault("State"));
            Assert.False(Convert.ToBoolean(result.ValidationBag.GetValueOrDefault("Resolved")));
            Assert.False(Convert.ToBoolean(result.ValidationBag.GetValueOrDefault("DPV")));
        }
    }

    [Fact]
    public async Task Valid_Standardized_Business_Address_Remove_StreetLine1_Successfully()
    {
        var request = new ValidateAddress(
            Guid.NewGuid().ToString(),
            new Address(
                "11435 W Buckeye Rd Ste 104-118",
                string.Empty,
                "AVONDALE",
                "AZ",
                "85323-6812",
                "US",
                false));

        foreach (var validator in _validators)
        {
            var result = await validator.ValidateAddressAsync(request);

            var proposed = result.ProposedAddress;
            Assert.NotNull(proposed);
            Assert.Equal("11435 W BUCKEYE RD", proposed?.StreetLine);
            Assert.Equal("STE 104-118", proposed?.StreetLine2);
            Assert.Equal("85323-6812", proposed?.PostalCode);

            // BUSINESS, RESIDENTIAL
            // MIXED (If it is a multi-tenant based address and contains both business and residential units.)
            // UNKNOWN (If just a zip code is provided, Address Validation Service returns 'unknown' for the business/residential classification)
            Assert.Equal("UNKNOWN", result.ValidationBag.GetValueOrDefault("Classification"));

            // If the address returned includes the address state of "Standardized" and also if the attributes of Resolved = True,
            // DPV = True are present, then the address is likely a valid one.
            Assert.Equal("STANDARDIZED", result.ValidationBag.GetValueOrDefault("State"));
            Assert.True(Convert.ToBoolean(result.ValidationBag.GetValueOrDefault("Resolved")));
            Assert.True(Convert.ToBoolean(result.ValidationBag.GetValueOrDefault("DPV")));
        }
    }

    [Fact]
    public async Task Valid_Standardized_Resedintial_Address_Remove_StreetLine1_Successfully()
    {
        var request = new ValidateAddress(
            Guid.NewGuid().ToString(),
            new Address(
                "5 Hood Road",
                "Derry, NH 03038",
                "Derry",
                "IL",
                "03038",
                "US",
                false));

        foreach (var validator in _validators)
        {
            var result = await validator.ValidateAddressAsync(request);

            var proposed = result.ProposedAddress;

            Assert.NotNull(proposed);
            Assert.Equal("5 HOOD RD", proposed?.StreetLine);
            Assert.Equal(string.Empty, proposed?.StreetLine2);
            Assert.Equal("03038-2012", proposed?.PostalCode);

            // BUSINESS, RESIDENTIAL
            // MIXED (If it is a multi-tenant based address and contains both business and residential units.)
            // UNKNOWN (If just a zip code is provided, Address Validation Service returns 'unknown' for the business/residential classification)
            Assert.Equal("BUSINESS", result.ValidationBag.GetValueOrDefault("Classification"));

            // If the address returned includes the address state of "Standardized" and also if the attributes of Resolved = True,
            // DPV = True are present, then the address is likely a valid one.
            Assert.Equal("STANDARDIZED", result.ValidationBag.GetValueOrDefault("State"));
            Assert.True(Convert.ToBoolean(result.ValidationBag.GetValueOrDefault("Resolved")));
            Assert.True(Convert.ToBoolean(result.ValidationBag.GetValueOrDefault("DPV")));
        }
    }

    [Fact]
    public async Task Valid_Standardized_Resedintial_Address_Remove_Dup_StreetLine1_Successfully()
    {
        var request = new ValidateAddress(
            Guid.NewGuid().ToString(),
            new Address(
                "39 W210 E BURNHAM LN",
                "39 W210 E BURNHAM LN",
                "GENEVA",
                "IL",
                "60134",
                "US",
                false));

        foreach (var validator in _validators)
        {
            var result = await validator.ValidateAddressAsync(request);

            var proposed = result.ProposedAddress;

            Assert.NotNull(proposed);
            Assert.Equal("39W210 E BURNHAM LN", proposed?.StreetLine);
            Assert.Equal(string.Empty, proposed?.StreetLine2);
            Assert.Equal("60134-4915", proposed?.PostalCode);

            // BUSINESS, RESIDENTIAL
            // MIXED (If it is a multi-tenant based address and contains both business and residential units.)
            // UNKNOWN (If just a zip code is provided, Address Validation Service returns 'unknown' for the business/residential classification)
            Assert.Equal("RESIDENTIAL", result.ValidationBag.GetValueOrDefault("Classification"));

            // If the address returned includes the address state of "Standardized" and also if the attributes of Resolved = True,
            // DPV = True are present, then the address is likely a valid one.
            Assert.Equal("STANDARDIZED", result.ValidationBag.GetValueOrDefault("State"));
            Assert.True(Convert.ToBoolean(result.ValidationBag.GetValueOrDefault("Resolved")));
            Assert.True(Convert.ToBoolean(result.ValidationBag.GetValueOrDefault("DPV")));
        }
    }

    [Fact]
    public async Task Valid_Standardized_Resedintial_Address_Successfully2()
    {
        var request = new ValidateAddress(
            Guid.NewGuid().ToString(),
            new Address(
                "W2155 COUNTY HH",
                string.Empty,
                "MALONE",
                "NY",
                "53049",
                "US",
                false));

        foreach (var validator in _validators)
        {
            var result = await validator.ValidateAddressAsync(request);

            var proposed = result.ProposedAddress;

            Assert.NotNull(proposed);
            Assert.Equal("W2155 COUNTY ROAD HH", proposed?.StreetLine);
            Assert.Equal(string.Empty, proposed?.StreetLine2);

            // BUSINESS, RESIDENTIAL
            // MIXED (If it is a multi-tenant based address and contains both business and residential units.)
            // UNKNOWN (If just a zip code is provided, Address Validation Service returns 'unknown' for the business/residential classification)
            Assert.Equal("RESIDENTIAL", result.ValidationBag.GetValueOrDefault("Classification"));

            // If the address returned includes the address state of "Standardized" and also if the attributes of Resolved = True,
            // DPV = True are present, then the address is likely a valid one.
            Assert.Equal("STANDARDIZED", result.ValidationBag.GetValueOrDefault("State"));
            Assert.True(Convert.ToBoolean(result.ValidationBag.GetValueOrDefault("Resolved")));
            Assert.True(Convert.ToBoolean(result.ValidationBag.GetValueOrDefault("DPV")));
        }
    }

    [Fact]
    public async Task Valid_Standardized_Resedintial_Address_Successfully()
    {
        var request = new ValidateAddress(
            Guid.NewGuid().ToString(),
            new Address(
                "2139 45TH RD.",
                "FL 1",
                "LONG ISLAND CITY",
                "NY",
                "11101",
                "US",
                false));

        foreach (var validator in _validators)
        {
            var result = await validator.ValidateAddressAsync(request);

            var proposed = result.ProposedAddress;
            Assert.NotNull(proposed);

            Assert.Equal("2139 45TH RD", proposed?.StreetLine);
            Assert.Equal("FL 1", proposed?.StreetLine2);

            // BUSINESS, RESIDENTIAL
            // MIXED (If it is a multi-tenant based address and contains both business and residential units.)
            // UNKNOWN (If just a zip code is provided, Address Validation Service returns 'unknown' for the business/residential classification)
            Assert.Equal("RESIDENTIAL", result.ValidationBag.GetValueOrDefault("Classification"));

            // If the address returned includes the address state of "Standardized" and also if the attributes of Resolved = True,
            // DPV = True are present, then the address is likely a valid one.
            Assert.Equal("STANDARDIZED", result.ValidationBag.GetValueOrDefault("State"));
            Assert.True(Convert.ToBoolean(result.ValidationBag.GetValueOrDefault("Resolved")));
            Assert.True(Convert.ToBoolean(result.ValidationBag.GetValueOrDefault("DPV")));
        }
    }

    private IFedExAddressValidationProvider GetAddressValidator()
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

        services.AddWebServicesFedExAddressValidationProvider();

        var sp = services.BuildServiceProvider();
        return sp.GetRequiredService<IFedExAddressValidationProvider>();
    }
}
