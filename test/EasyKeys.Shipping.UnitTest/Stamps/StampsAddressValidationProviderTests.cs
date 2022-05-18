using System.Collections;
using System.ServiceModel;

using Bet.Extensions.Testing.Logging;

using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Services;
using EasyKeys.Shipping.Stamps.Abstractions.Services.Impl;
using EasyKeys.Shipping.Stamps.AddressValidation;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Moq;

using StampsClient.v111;

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
        EasyKeys.Shipping.Abstractions.Models.Address address,
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

    [Theory]
    [InlineData("Conversation out-of-sync.")]
    public async Task Address_Validation_Handles_Exceptions_Successfully(string exMessage)
    {
        // arrange
        var stampsClientMock = new Mock<IStampsClientService>();

        var swsimV111Mock = new Mock<SwsimV111Soap>();

        stampsClientMock.Setup(x => x.RefreshTokenAsync(It.IsAny<CancellationToken>()))
            .Verifiable();

        swsimV111Mock.Setup(x => x.CleanseAddressAsync(It.IsAny<CleanseAddressRequest>()))
            .Verifiable();

        swsimV111Mock.SetupSequence(x => x.CleanseAddressAsync(It.IsAny<CleanseAddressRequest>()))
            .ThrowsAsync(new FaultException(exMessage))
            .ThrowsAsync(new Exception(exMessage));

        stampsClientMock.Setup(x => x.CreateClient()).Returns(swsimV111Mock.Object);

        var stampsAddressValidationProvider = new StampsAddressValidationProvider(stampsClientMock.Object, new PolicyService(stampsClientMock.Object));

        var validateAddress = new ValidateAddress(
            "test",
            new EasyKeys.Shipping.Abstractions.Models.Address()
            {
                StreetLine = "1550 Central Ave",
                StreetLine2 = "Apt 35",
                City = "Riverside",
                StateOrProvince = "CA",
                CountryCode = "US",
                PostalCode = "92507"
            });

        // act
        var result = await stampsAddressValidationProvider.ValidateAddressAsync(validateAddress, CancellationToken.None);

        stampsClientMock.Verify(x => x.RefreshTokenAsync(It.IsAny<CancellationToken>()), Times.Exactly(1));

        swsimV111Mock.Verify(x => x.CleanseAddressAsync(It.IsAny<CleanseAddressRequest>()), Times.Exactly(2));

        // assert
        Assert.IsType<ValidateAddress>(result);

        Assert.Contains(result.InternalErrors, x => x == exMessage);
    }

    [Theory]
    [InlineData("Conversation out-of-sync.")]
    public async Task Address_Validation_Refreshes_Token_And_Returns_Address_Successfully(string exMessage)
    {
        // arrange
        var stampsClientMock = new Mock<IStampsClientService>();

        var swsimV111Mock = new Mock<SwsimV111Soap>();

        stampsClientMock.Setup(x => x.RefreshTokenAsync(It.IsAny<CancellationToken>()))
            .Verifiable();

        swsimV111Mock.Setup(x => x.CleanseAddressAsync(It.IsAny<CleanseAddressRequest>()))
            .Verifiable();

        swsimV111Mock.SetupSequence(x => x.CleanseAddressAsync(It.IsAny<CleanseAddressRequest>()))
            .ThrowsAsync(new FaultException(exMessage))
            .ReturnsAsync(new CleanseAddressResponse()
            { Authenticator = "test", CandidateAddresses = new StampsClient.v111.Address[0], Address = new StampsClient.v111.Address() });

        stampsClientMock.Setup(x => x.CreateClient()).Returns(swsimV111Mock.Object);

        var stampsAddressValidationProvider = new StampsAddressValidationProvider(stampsClientMock.Object, new PolicyService(stampsClientMock.Object));

        var validateAddress = new ValidateAddress(
            "test",
            new EasyKeys.Shipping.Abstractions.Models.Address()
            {
                StreetLine = "1550 Central Ave",
                StreetLine2 = "Apt 35",
                City = "Riverside",
                StateOrProvince = "CA",
                CountryCode = "US",
                PostalCode = "92507"
            });

        // act
        var result = await stampsAddressValidationProvider.ValidateAddressAsync(validateAddress, CancellationToken.None);

        stampsClientMock.Verify(x => x.RefreshTokenAsync(It.IsAny<CancellationToken>()), Times.Exactly(1));

        swsimV111Mock.Verify(x => x.CleanseAddressAsync(It.IsAny<CleanseAddressRequest>()), Times.Exactly(2));

        // assert
        Assert.IsType<ValidateAddress>(result);
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
                 new EasyKeys.Shipping.Abstractions.Models.Address()
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
                 new EasyKeys.Shipping.Abstractions.Models.Address()
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
                 new EasyKeys.Shipping.Abstractions.Models.Address()
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
                 new EasyKeys.Shipping.Abstractions.Models.Address()
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
                 new EasyKeys.Shipping.Abstractions.Models.Address()
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
