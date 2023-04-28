using System.Collections;

using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.AddressValidation;

using EasyKeysShipping.FuncTest.TestHelpers;

using Microsoft.Extensions.DependencyInjection;

namespace EasyKeysShipping.FuncTest.Stamps;

public class StampsAddressValidationProviderTests
{
    private readonly IStampsAddressValidationProvider _validator;

    public StampsAddressValidationProviderTests(ITestOutputHelper output)
    {
        _validator = ServiceProviderInstance.GetStampsServices(output)
            .GetRequiredService<IStampsAddressValidationProvider>();
    }

    [Theory]
    [ClassData(typeof(AddressTestData))]
    public async Task Address_Validation_Successfully(
        Address address,
        int errorCount,
        int internalErrorCount,
        bool cityStateZipOk,
        bool addressMatch,
        string validationResult)
    {
        var cancellationToken = CancellationToken.None;

        var request = new ValidateAddress(
            Guid.NewGuid().ToString(),
            address);

        var result = await _validator.ValidateAddressAsync(request, cancellationToken);

        Assert.NotNull(result);

        Assert.Equal(internalErrorCount, result.InternalErrors.Count);

        Assert.Equal(errorCount, result.Errors.Count);

        Assert.Equal(Convert.ToBoolean(result.ValidationBag["CityStateZipOK"]), cityStateZipOk);
        Assert.Equal(Convert.ToBoolean(result.ValidationBag["AddressMatch"]), addressMatch);
        Assert.Equal(result.ValidationBag["ValidationResult"], validationResult);
    }

    private class AddressTestData : IEnumerable<object[]>
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

                 // CityStateZipOK
                 true,

                 // AddressMatch
                 true,

                 // ValidationResult
                 "Full Address Verified."
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

                 // CitStateZipOK
                 false,

                 // AddressMatch
                 false,

                 // ValidationResult
                 "An address must be specified in order to [cleanse an address./print a label.] Exception with code 0x0065010e; module 101, category 1, item 14"
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

                 // CityStateZipOK
                 true,

                 // AddressMatch
                 false,

                 // ValidationResult
                 "City, State, and Zip are valid, but the Street could not be verified."
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

                 // CityStateZipOK
                 true,

                 // AddressMatch
                 true,

                 // ValidationResult
                 "To Country Verified."
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

                 // CityStateZipOK
                 true,

                 // AddressMatch
                 true,

                 // ValidationResult
                 "To Country Verified."
            };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
