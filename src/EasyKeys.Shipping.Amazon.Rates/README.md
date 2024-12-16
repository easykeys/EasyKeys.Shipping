# EasyKeys.Shipping.Amazon.Rates

[![Build status](https://ci.appveyor.com/api/projects/status/xp52rbpa9vmr1ck9?svg=true)](https://ci.appveyor.com/project/easykeys/easykeys-shipping)
[![NuGet](https://img.shields.io/nuget/v/EasyKeys.Shipping.Amazon.Rates.svg)](https://www.nuget.org/packages?q=EasyKeys.Shipping.Amazon.Rates)
![Nuget](https://img.shields.io/nuget/dt/EasyKeys.Shipping.Amazon.Rates)
[![feedz.io](https://img.shields.io/badge/endpoint.svg?url=https://f.feedz.io/easykeys/core/shield/EasyKeys.Shipping.Amazon.Rates/latest)](https://f.feedz.io/easykeys/core/packages/EasyKeys.Shipping.Amazon.Rates/latest/download)

AmazonShippingRateProvider is a lightweight .NET Core library that provides an easy-to-use wrapper around Amazon's Shipping API endpoints for fetching and managing shipping rates. This library simplifies integration with Amazon's shipping services, allowing developers to interact with the API using clean and intuitive methods.

## Features

- Fetch shipping rates for different carrier services.
- Support for international and domestic shipping rate queries.
- Easy-to-configure settings for Amazon API credentials.
- Built-in error handling and validation.
- Fully asynchronous API methods for seamless integration.

## Installation

Install the package via NuGet Package Manager:

```bash
Install-Package EasyKeys.Shipping.Amazon.Rates
```

Or via the .NET CLI:

```bash
 dotnet add package EasyKeys.Shipping.Amazon.Rates
```

## Prerequisites

Before using this library, ensure you have the following:

- .NET Core 6.0 or later.
- Amazon Shipping API credentials (Access Key, Secret Key).
- An active Amazon Seller or Partner account with Shipping API access.

## Getting Started

### 1. Configure the Library

Add your Amazon API credentials to your application's configuration file (e.g., `appsettings.json`):

```json
{
  "AmazonShipping": {
    "ClientId": "YourClientId",
    "ClientSecret": "YourClientSecret",
    "RefreshToken": "YourRefreshToken",
    "IsDevelopment": true
  }
}
```

Alternatively, you can pass these credentials programmatically.

### 2. Initialize the Provider

Create an instance of the `AmazonShippingRateProvider` in your application:

```csharp
using AmazonShippingRateProvider;

var options = new AmazonShippingApiOptions
{
    ClientId = "YourClientId",
    ClientSecret = "YourClientSecret",
    RefreshToken = "YourRefreshToken",
    IsDevelopment = true
};

var authenticatorService = new AmazonApiAuthenticatorService(); // Your implementation
var shippingApi = new AmazonShippingApi(); // API client
var logger = new LoggerFactory().CreateLogger<AmazonShippingRateProvider>();

var rateProvider = new AmazonShippingRateProvider(logger, options, authenticatorService, shippingApi);
```

### 3. Fetch Shipping Rates

Call the `GetRatesAsync` method with the necessary parameters:

```csharp
var shipment = new Shipment
{
    OriginAddress = new Address
    {
        StreetLine = "123 Origin St",
        City = "OriginCity",
        StateOrProvince = "NC",
        CountryCode = "US",
        PostalCode = "12345"
    },
    DestinationAddress = new Address
    {
        StreetLine = "456 Destination St",
        City = "DestinationCity",
        StateOrProvince = "CA",
        CountryCode = "US",
        PostalCode = "67890"
    },
    Packages = new List<Package>
    {
        new Package
        {
            Dimensions = new Dimensions
            {
                RoundedLength = 10,
                RoundedWidth = 5,
                RoundedHeight = 5
            },
            RoundedWeight = 1.5M,
            InsuredValue = 100.00M
        }
    },
    Options = new ShipmentOptions
    {
        ShippingDate = DateTime.UtcNow
    }
};

var rates = await rateProvider.GetRatesAsync(shipment);

foreach (var rate in shipment.Rates)
{
    Console.WriteLine($"Carrier: {rate.Carrier}, Service: {rate.Service}, Price: {rate.Price}");
}
```

## Error Handling

The library throws exceptions for the following scenarios:
- `ApiException<ErrorList>`: When the API returns an error response with detailed errors.
- `ApiException`: When there is a general API error.
- `Exception`: For unexpected errors.

Wrap your calls in try-catch blocks for production use:

```csharp
try
{
    var rates = await rateProvider.GetRatesAsync(shipment);
    // Process rates
}
catch (ApiException<ErrorList> ex)
{
    Console.WriteLine($"API Error: {string.Join(",", ex.Result.Errors.Select(e => e.Message))}");
}
catch (ApiException ex)
{
    Console.WriteLine($"API Error: {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"Unexpected Error: {ex.Message}");
}
```

## Implementation Details

### Dependencies

The `AmazonShippingRateProvider` depends on the following services:

- **`IAmazonApiAuthenticatorService`**: Handles the retrieval of authentication tokens for the Amazon Shipping API.
- **`AmazonShippingApiOptions`**: Provides configuration options such as client ID, client secret, and environment details.
- **`AmazonShippingApi`**: The underlying API client used to interact with Amazon's endpoints.
- **`ILogger<AmazonShippingRateProvider>`**: Used for logging purposes.

### Initialization

The constructor of `AmazonShippingRateProvider` takes the following parameters:

- `ILogger<AmazonShippingRateProvider> logger`
- `AmazonShippingApiOptions options`
- `IAmazonApiAuthenticatorService authenticatorService`
- `AmazonShippingApi shippingApi`

### API Flow

1. **Create `GetRatesRequest`:** Constructs the request object based on shipment details.
2. **Authenticate:** Retrieves an authentication token using `IAmazonApiAuthenticatorService`.
3. **Send Request:** Calls `AmazonShippingApi.GetRatesAsync` with the token and request.
4. **Parse Response:** Processes the API response, converting rates to the internal `Rate` model and attaching them to the shipment object.

## Contributing

Contributions are welcome! Feel free to open an issue or submit a pull request if you have suggestions or improvements.

### Steps to Contribute:
1. Fork the repository.
2. Create a new branch for your feature or bug fix.
3. Commit your changes with descriptive messages.
4. Submit a pull request for review.

## License

This library is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.

---

For any issues or questions, please contact [devs@easykeys.com](mailto:devs@easykeys.com).

