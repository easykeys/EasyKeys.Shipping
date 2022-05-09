using EasyKeys.Shipping.Abstractions;
using EasyKeys.Shipping.Abstractions.Extensions;
using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.FedEx.Abstractions.Models;
using EasyKeys.Shipping.FedEx.Abstractions.Options;
using EasyKeys.Shipping.FedEx.Abstractions.Services;
using EasyKeys.Shipping.FedEx.Extensions;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using RateClient.v28;

namespace EasyKeys.Shipping.FedEx.Rates;

public class FedExRateProvider : IFedExRateProvider
{
    private readonly FedExOptions _options;
    private readonly RatePortType _rateClient;
    private readonly ILogger<FedExRateProvider> _logger;

    public FedExRateProvider(
        IOptionsSnapshot<FedExOptions> options,
        IFedExClientService fedExClientService,
        ILogger<FedExRateProvider> logger)
    {
        _options = options.Value;
        _rateClient = fedExClientService.CreateRateClient();
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Shipment> GetRatesAsync(
        Shipment shipment,
        ServiceType serviceType = ServiceType.DEFAULT,
        CancellationToken cancellationToken = default)
    {
        var client = _rateClient;

        try
        {
            var request = CreateRateRequest(shipment, serviceType);

            var serviceRequest = new getRatesRequest(request);

            var reply = await client.getRatesAsync(serviceRequest);

            if (reply.RateReply != null)
            {
                ProcessReply(reply.RateReply, shipment);
                ProcessErrors(reply.RateReply, shipment);
            }
            else
            {
                _logger.LogError("{providerName}: API returned NULL result", nameof(FedExRateProvider));
                shipment.InternalErrors.Add("FedEx provider: API returned NULL result");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{providerName} failed", nameof(FedExRateProvider));
            shipment.InternalErrors.Add(ex?.Message ?? $"{nameof(FedExRateProvider)} failed");
        }

        return shipment;
    }

    private RateRequest CreateRateRequest(Shipment shipment, ServiceType serviceType)
    {
        // Build the RateRequest
        var request = new RateRequest
        {
            WebAuthenticationDetail = new WebAuthenticationDetail
            {
                UserCredential = new WebAuthenticationCredential
                {
                    Key = _options.FedExKey,
                    Password = _options.FedExPassword
                }
            },

            ClientDetail = new ClientDetail
            {
                AccountNumber = _options.FedExAccountNumber,
                MeterNumber = _options.FedExMeterNumber
            },

            Version = new VersionId(),
            ReturnTransitAndCommit = true,
            ReturnTransitAndCommitSpecified = true,
            RequestedShipment = new RequestedShipment()
            {
                ShipTimestamp = shipment.Options.ShippingDate, // Shipping date and time
                ShipTimestampSpecified = true,
                DropoffType = DropoffType.REGULAR_PICKUP,           // Drop off types are BUSINESS_SERVICE_CENTER, DROP_BOX, REGULAR_PICKUP, REQUEST_COURIER, STATION
                DropoffTypeSpecified = true,
                PackagingType = shipment.Options.PackagingType,     // "YOUR_PACKAGING",
                PackageCount = shipment.Packages.Count.ToString(),
                RateRequestTypes = GetRateRequestTypes().ToArray(),
                PreferredCurrency = shipment.Options.GetCurrencyCode(),
                TotalInsuredValue = new Money
                {
                    Amount = shipment.Packages.Sum(x => x.InsuredValue),
                    AmountSpecified = true,
                }
            }
        };

        if (shipment.DestinationAddress.IsUnitedStatesAddress())
        {
            request.RequestedShipment.TotalInsuredValue.Currency = shipment.Options.PreferredCurrencyCode;
        }

        if (serviceType != ServiceType.DEFAULT)
        {
            request.RequestedShipment.ServiceType = serviceType.ToString();
        }

        if (shipment.Options.SaturdayDelivery)
        {
            // include Saturday delivery
            request.VariableOptions = new[] { ServiceOptionType.SATURDAY_DELIVERY };
        }

        SetShipmentDetails(request, shipment);

        return request;
    }

    private void SetShipmentDetails(RateRequest request, Shipment shipment)
    {
        SetOrigin(request, shipment);
        SetDestination(request, shipment);
        SetPackageLineItems(request, shipment);
    }

    private void SetDestination(RateRequest request, Shipment shipment)
    {
        request.RequestedShipment.Recipient = new Party
        {
            Address = shipment.DestinationAddress.GetFedExAddress()
        };
    }

    private void SetOrigin(RateRequest request, Shipment shipment)
    {
        request.RequestedShipment.Shipper = new Party
        {
            Address = shipment.OriginAddress.GetFedExAddress()
        };
    }

    private void SetPackageLineItems(RateRequest request, Shipment shipment)
    {
        request.RequestedShipment.RequestedPackageLineItems = new RequestedPackageLineItem[shipment.Packages.Count];

        var i = 0;
        foreach (var package in shipment.Packages)
        {
            request.RequestedShipment.RequestedPackageLineItems[i] = new RequestedPackageLineItem()
            {
                SequenceNumber = (i + 1).ToString(),
                GroupPackageCount = "1",

                // Package weight
                Weight = new Weight()
                {
                    Units = WeightUnits.LB,
                    UnitsSpecified = true,
                    Value = package.RoundedWeight,
                    ValueSpecified = true
                },

                // Package dimensions
                Dimensions = new RateClient.v28.Dimensions()
                {
                    Length = package.Dimensions.RoundedLength.ToString(),
                    Width = package.Dimensions.RoundedWidth.ToString(),
                    Height = package.Dimensions.RoundedHeight.ToString(),
                    Units = LinearUnits.IN,
                    UnitsSpecified = true
                }
            };

            // package insured value
            request.RequestedShipment.RequestedPackageLineItems[i].InsuredValue = new Money
            {
                Amount = package.InsuredValue,
                AmountSpecified = package.InsuredValue > 0,
                Currency = shipment.Options.GetCurrencyCode(),
            };

            if (package.SignatureRequiredOnDelivery)
            {
                var signatureOptionDetail = new SignatureOptionDetail { OptionType = SignatureOptionType.INDIRECT };
                request.RequestedShipment.RequestedPackageLineItems[i].SpecialServicesRequested = new PackageSpecialServicesRequested() { SignatureOptionDetail = signatureOptionDetail };
            }

            i++;
        }
    }

    private IEnumerable<RateRequestType> GetRateRequestTypes()
    {
        yield return RateRequestType.LIST;
    }

    /// <summary>
    /// Processes the reply.
    /// </summary>
    /// <param name="reply"></param>
    /// <param name="shipment"></param>
    private void ProcessReply(RateReply reply, Shipment shipment)
    {
        if (reply?.RateReplyDetails == null)
        {
            return;
        }

        foreach (var rateReplyDetail in reply.RateReplyDetails)
        {
            var name = rateReplyDetail.ServiceType;
            var saturdayDelievery = rateReplyDetail?.AppliedOptions?.Contains(ServiceOptionType.SATURDAY_DELIVERY) ?? false;

            var netCost = rateReplyDetail!.RatedShipmentDetails.FirstOrDefault(r => r.ShipmentRateDetail.RateType == ReturnedRateType.PAYOR_ACCOUNT_PACKAGE
                    || r.ShipmentRateDetail.RateType == ReturnedRateType.PAYOR_ACCOUNT_SHIPMENT)?.ShipmentRateDetail.TotalNetCharge.Amount;

            var listCost = rateReplyDetail.RatedShipmentDetails.FirstOrDefault(r => r.ShipmentRateDetail.RateType == ReturnedRateType.PAYOR_LIST_PACKAGE
                    || r.ShipmentRateDetail.RateType == ReturnedRateType.PAYOR_LIST_SHIPMENT)?.ShipmentRateDetail.TotalNetCharge.Amount;

            var guaranteedDelivery = rateReplyDetail.DeliveryTimestamp;
            if (guaranteedDelivery == DateTime.MinValue)
            {
                var shipDate = shipment.Options.ShippingDate;

                var map = new Dictionary<string, int>() { { "one", 1 }, { "two", 2 }, { "three", 3 }, { "four", 4 }, { "five", 5 }, { "six", 6 }, { "seven", 7 }, { "eight", 8 }, { "nine", 9 }, { "ten", 10 }, { "eleven", 11 }, { "twelve", 12 }, { "thirteen", 13 }, { "fourteen", 14 }, { "fifteen", 15 }, { "sixteen", 16 }, { "seventeen", 17 }, { "eighteen", 18 }, { "nineteen", 19 }, { "twenty", 20 } };

                var t = rateReplyDetail.TransitTime.ToString().Split('_');
                if (map.TryGetValue(t[0].ToLower(), out var index))
                {
                    guaranteedDelivery = shipDate.AddBusinessDays(index + 1);
                }

                // FEDEX_INTERNATIONAL_PRIORITY
                if (name == nameof(ServiceType.INTERNATIONAL_PRIORITY))
                {
                    guaranteedDelivery = shipDate.AddBusinessDays(3);
                }
                else if (name == nameof(ServiceType.INTERNATIONAL_ECONOMY))
                {
                    guaranteedDelivery = shipDate.AddBusinessDays(6);
                }
                else if (name == nameof(ServiceType.FEDEX_GROUND))
                {
                    guaranteedDelivery = shipDate.AddBusinessDays(1);
                }
            }

            // var deliveryDate = rateReplyDetail.DeliveryTimestampSpecified ? rateReplyDetail.DeliveryTimestamp : DateTime.Now.AddDays(30);
            var uName = name.Replace("_", " ");
            if (!uName.Contains("FEDEX"))
            {
                uName = $"FEDEX {uName}";
            }

            var rate = new Rate(
                uName,
                name,
                netCost ?? 0.0M,
                listCost ?? 0.0M,
                guaranteedDelivery,
                saturdayDelievery,
                shipment.Options.GetCurrencyCode());

            shipment.Rates.Add(rate);
        }
    }

    private void ProcessErrors(RateReply reply, Shipment shipment)
    {
        var errorTypes = new NotificationSeverityType[]
        {
                NotificationSeverityType.ERROR,
                NotificationSeverityType.FAILURE
        };

        var noReplyDetails = reply.RateReplyDetails == null;

        if (reply.Notifications != null && reply.Notifications.Any())
        {
            var errors = reply.Notifications
                .Where(e => !e.SeveritySpecified || errorTypes.Contains(e.Severity) || noReplyDetails)
                .Select(error =>
                new Error
                {
                    Description = error.Message,
                    Source = error.Source,
                    Number = error.Code
                });

            foreach (var err in errors)
            {
                shipment.Errors.Add(err);
            }
        }
    }
}
