# EasyKeys.Shipping.Amazon.Shipment

[![Build status](https://ci.appveyor.com/api/projects/status/xp52rbpa9vmr1ck9?svg=true)](https://ci.appveyor.com/project/easykeys/easykeys-shipping)
[![NuGet](https://img.shields.io/nuget/v/EasyKeys.Shipping.Amazon.Shipment.svg)](https://www.nuget.org/packages?q=EasyKeys.Shipping.Amazon.Shipment)
![Nuget](https://img.shields.io/nuget/dt/EasyKeys.Shipping.Amazon.Shipment)
[![feedz.io](https://img.shields.io/badge/endpoint.svg?url=https://f.feedz.io/easykeys/core/shield/EasyKeys.Shipping.Amazon.Shipment/latest)](https://f.feedz.io/easykeys/core/packages/EasyKeys.Shipping.Amazon.Shipment/latest/download)

AmazonShippingShipmentProvider is a .NET Core library designed to simplify interactions with Amazon's Shipping API for creating, managing, and canceling shipments. It provides a streamlined API interface for handling shipments and generating shipping labels.

## Features

- Create shipments and generate shipping labels.
- Cancel existing shipments.
- Flexible support for label formats (PNG, PDF).
- Supports asynchronous operations for better performance.
- Built-in error handling with detailed logging.

## Installation

Install the package via NuGet Package Manager:

```bash
Install-Package EasyKeys.Shipping.Amazon.Shipment
```

Or via the .NET CLI:

```bash
 dotnet add package EasyKeys.Shipping.Amazon.Shipment
```

## Prerequisites

Before using this library, ensure you have the following:

- .NET Core 6.0 or later.
- Amazon Shipping API credentials (Client ID, Client Secret, and Refresh Token).
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

Create an instance of the `AmazonShippingShipmentProvider` in your application:

```csharp
using AmazonShippingShipmentProvider;

var options = new AmazonShippingApiOptions
{
    ClientId = "YourClientId",
    ClientSecret = "YourClientSecret",
    RefreshToken = "YourRefreshToken",
    IsDevelopment = true
};

var authenticatorService = new AmazonApiAuthenticatorService(); // Your implementation
var shippingApi = new AmazonShippingApi(); // API client
var logger = new LoggerFactory().CreateLogger<AmazonShippingShipmentProvider>();

var shipmentProvider = new AmazonShippingShipmentProvider(logger, options, shippingApi, authenticatorService);
```

### 3. Create a Smart Shipment

Call the `CreateSmartShipmentAsync` method to generate a shipment and its associated label:

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

var shippingDetails = new ShippingDetails
{
    Recipient = new Contact
    {
        FullName = "John Doe",
        Email = "john.doe@example.com",
        PhoneNumber = "1234567890",
        Company = "Recipient Company"
    },
    Sender = new Contact
    {
        FullName = "Jane Doe",
        Email = "jane.doe@example.com",
        PhoneNumber = "9876543210",
        Company = "Sender Company"
    },
    ServiceId = "ServiceId123",
    LabelFormat = "PDF",
    LabelDimensions = new Dimensions { Length = 8.5, Width = 11 },
    LabelUnit = "INCH",
    LabelDpi = 300
};

var shipmentLabel = await shipmentProvider.CreateSmartShipmentAsync(shipment, shippingDetails);

foreach (var label in shipmentLabel.Labels)
{
    Console.WriteLine($"Label Tracking ID: {label.TrackingId}, Net Charge: {label.TotalCharges.NetCharge}");
}
```

### 4. Cancel a Shipment

To cancel an existing shipment, call the `CancelShipmentAsync` method:

```csharp
var cancellationResult = await shipmentProvider.CancelShipmentAsync("ShipmentId123");

if (cancellationResult.Errors.Any())
{
    Console.WriteLine("Failed to cancel shipment: " + string.Join(",", cancellationResult.Errors));
}
else
{
    Console.WriteLine("Shipment canceled successfully.");
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
    var shipmentLabel = await shipmentProvider.CreateSmartShipmentAsync(shipment, shippingDetails);
    // Process labels
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

The `AmazonShippingShipmentProvider` depends on the following services:

- **`IAmazonApiAuthenticatorService`**: Handles the retrieval of authentication tokens for the Amazon Shipping API.
- **`AmazonShippingApiOptions`**: Provides configuration options such as client ID, client secret, and environment details.
- **`AmazonShippingApi`**: The underlying API client used to interact with Amazon's endpoints.
- **`ILogger<AmazonShippingShipmentProvider>`**: Used for logging purposes.

### API Flow

1. **Create `OneClickShipmentRequest`:** Constructs the request object based on shipment details and shipping options.
2. **Authenticate:** Retrieves an authentication token using `IAmazonApiAuthenticatorService`.
3. **Send Request:** Calls `AmazonShippingApi.OneClickShipmentAsync` to create the shipment.
4. **Parse Response:** Processes the API response, extracting labels and attaching them to the shipment object.

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

