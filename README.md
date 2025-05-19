# EasyKeys.Shipping

[![Build status](https://ci.appveyor.com/api/projects/status/xp52rbpa9vmr1ck9?svg=true)](https://ci.appveyor.com/project/easykeys/easykeys-shipping)
[![NuGet](https://img.shields.io/nuget/v/EasyKeys.Shipping.Abstractions.svg)](https://www.nuget.org/packages?q=EasyKeys.Shipping.Abstractions)
![Nuget](https://img.shields.io/nuget/dt/EasyKeys.Shipping.Abstractions)
[![feedz.io](https://img.shields.io/badge/endpoint.svg?url=https://f.feedz.io/easykeys/core/shield/EasyKeys.Shipping.Abstractions/latest)](https://f.feedz.io/easykeys/core/packages/EasyKeys.Shipping.Abstractions/latest/download)

[EasyKeys.com](https://easykeys.com) production ready shipment library for DHL Express, Amazon, FedEx, Stamps and USPS shipping providers.

## Give a Star! :star:

If you like or are using this project please give it a star. Thanks!

## Reusable Abstractions

- [x] [EasyKeys.Shipping.Abstractions](./src/EasyKeys.Shipping.Abstractions)
- [x] [EasyKeys.Shipping.PostalAddress](./src/EasyKeys.Shipping.PostalAddress)

## DHL Shipping

- [x] [EasyKeys.Shipping.DHL.Abstractions](./src/EasyKeys.Shipping.DHL.Abstractions)
- [x] [EasyKeys.Shipping.DHL.AddressValidation](./src/EasyKeys.Shipping.DHL.AddressValidation)
- [x] [EasyKeys.Shipping.DHL.Rates](./src/EasyKeys.Shipping.DHL.Rates)
- [x] [EasyKeys.Shipping.DHL.Shipment](./src/EasyKeys.Shipping.DHL.Shipment)

## Amazon Shipping

- [x] [EasyKeys.Shipping.Amazon.Abstractions](./src/EasyKeys.Shipping.Amazon.Abstractions)
- [x] [EasyKeys.Shipping.Amazon.Rates](./src/EasyKeys.Shipping.Amazon.Rates)
- [x] [EasyKeys.Shipping.Amazon.Shipment](./src/EasyKeys.Shipping.Amazon.Shipment)

## FedEx Shipping

- [x] [EasyKeys.Shipping.FedEx.Abstractions](./src/EasyKeys.Shipping.FedEx.Abstractions)
- [x] [EasyKeys.Shipping.FedEx.AddressValidation](./src/EasyKeys.Shipping.FedEx.AddressValidation)
- [x] [EasyKeys.Shipping.FedEx.Rates](./src/EasyKeys.Shipping.FedEx.Rates)
- [x] [EasyKeys.Shipping.FedEx.Shipment](./src/EasyKeys.Shipping.FedEx.Shipment)
- [x] [EasyKeys.Shipping.FedEx.Tracking](./src/EasyKeys.Shipping.FedEx.Tracking)
- [x] [EasyKeys.Shipping.FedEx.Console](./src/EasyKeys.Shipping.FedEx.Console) - Sample Application

## Stamps Shipping

- [x] [EasyKeys.Shipping.Stamps.Abstractions](./src/EasyKeys.Shipping.Stamps.Abstractions)
- [x] [EasyKeys.Shipping.Stamps.AddressValidation](./src/EasyKeys.Shipping.Stamps.AddressValidation)
- [x] [EasyKeys.Shipping.Stamps.Rates](./src/EasyKeys.Shipping.Stamps.Rates)
- [x] [EasyKeys.Shipping.Stamps.Shipment](./src/EasyKeys.Shipping.Stamps.Shipment)
- [x] [EasyKeys.Shipping.Stamps.Tracking](./src/EasyKeys.Shipping.Stamps.Tracking)
- [x] [EasyKeys.Shipping.Stamps.Console](./src/EasyKeys.Shipping.Stamps.Console) - Sample application

## USPS Shipping

- [x] [EasyKeys.Shipping.Usps.Abstractions](./src/EasyKeys.Shipping.Usps.Abstractions)
- [x] [EasyKeys.Shipping.Usps.Rates](./src/EasyKeys.Shipping.Usps.Rates)

## Poly policy Sample

```csharp

    var request = new ValidateAddress(model?.RequestId ?? Guid.NewGuid().ToString(), originalAddress, originalAddress);

    var response = await _policy.ExecuteAsync(
        async (ctx, ct) => await _validationProvider.ValidateAddressAsync(request, ct),
        new Context
        {
            [_policyContextMethod] = "ValidateAsync"
        },
        cancellationToken);

    private IAsyncPolicy GetRetryWithTimeOutPolicy()
    {
        // each call is limited to 30 seconds in case when fedex is non-reponsive and it is 1min timeout, it is way to long
        // The request channel timed out attempting to send after 00:01:00.
        // Increase the timeout value passed to the call to Request or increase the SendTimeout value on the Binding. The time allotted to this operation may have been a portion of a longer timeout.
        var timeoutPolicy = Policy.TimeoutAsync(30, TimeoutStrategy.Pessimistic);

        var jitterer = new Random();

        return Policy
            .Handle<TimeoutRejectedException>()
            .WaitAndRetryAsync(
                  retryCount: 3,    // exponential back-off plus some jitter
                  sleepDurationProvider: (retryAttempt, context) =>
                  {
                      return TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                            + TimeSpan.FromMilliseconds(jitterer.Next(0, 200));
                  },
                  onRetry: (ex, span, context) =>
                  {
                      var methodName = context[_policyContextMethod] ?? "MethodNotSpecified";

                      _logger.LogWarning(
                        "{Method} wait {Seconds} to execute with exception: {Message} for named policy: {Policy}",
                        methodName,
                        span.TotalSeconds,
                        ex.Message,
                        context.PolicyKey);
                  })
             .WithPolicyKey($"{nameof(FedExAddressValidationProvider)}WaitAndRetryAsync")
             .WrapAsync(timeoutPolicy);
    }

```

## References

- https://github.com/leoboles/Integration.Stamps.git
- https://github.com/oparamo/StampsService.git
