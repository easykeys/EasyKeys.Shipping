
using System.Text.Json;

using EasyKeys.Shipping.Amazon.Abstractions.OpenApis.V2.Shipping;

try
{
    var client = new HttpClient();
    var request = new HttpRequestMessage
    {
        Method = HttpMethod.Post,
        RequestUri = new Uri("https://api.amazon.com/auth/o2/token"),
        Headers =
    {
        { "accept", "application/json" },
    },
        Content = new FormUrlEncodedContent(new Dictionary<string, string>
    {
        { "grant_type", "refresh_token" },
        { "refresh_token", "" },
        { "client_id", "" },
        { "client_secret", "" }
    }),
    };
    var accessToken = new Token("", "", 0);
    using (var response = await client.SendAsync(request))
    {
        //Console.WriteLine(response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        accessToken = JsonSerializer.Deserialize<Token>(body);
        //Console.WriteLine(body);
    }

    var rateRequest = new GetRatesRequest()
    {
        ShipDate = DateTimeOffset.Now.AddDays(2).ToString("yyyy-MM-dd'T'HH:mm:ss'Z'"),
        ShipTo = new Address()
        {
            Name = "test moffett",
            AddressLine1 = "22322 Roxford St",
            StateOrRegion = "Michigan",
            City = "Detroit",
            CountryCode = "United States",
            PostalCode = "48219-2381",
            Email = "test@gmail.com"
        },
        ShipFrom = new Address()
        {
            Name = "test moffett",
            AddressLine1 = "662 Broadway 3fl",
            StateOrRegion = "New York",
            City = "Brooklyn",
            CountryCode = "United States",
            PostalCode = "11206-4410",
            Email = "testfrom@gmail.com"
        },
        Packages = new()
        {
            new()
            {
                Dimensions = new()
                {
                    Unit = DimensionsUnit.INCH,
                    Length = 1,
                    Width = 1,
                    Height = 1
                },
                Weight = new()
                {
                    Unit = WeightUnit.OUNCE,
                    Value = 1
                },
                InsuredValue = new()
                {
                    Value = 1,
                    Unit = "USD"
                },
                PackageClientReferenceId = "packageClientReferenceId",
                Items = new()
                {
                    new()
                    {
                        Weight = new()
                        {
                            Unit = WeightUnit.GRAM
                        },
                        LiquidVolume = new()
                        {
                            Unit = LiquidVolumeUnit.ML
                        },
                        Description = "asdf",
                        Quantity = 1
                    }
                }
            }
        },
        ChannelDetails = new()
        {
            ChannelType = ChannelType.EXTERNAL
        }
    };

    var shippingApi = new AmazonShippingApi(new HttpClient());
    var result = await shippingApi.GetRatesAsync(accessToken.access_token, XAmznShippingBusinessId.AmazonShipping_US, rateRequest);
}
catch (ApiException<ErrorList> ex)
{
    Console.WriteLine($"Exception: {ex.Message}");
}

public record Token(string access_token, string token_type, int expires_in);
