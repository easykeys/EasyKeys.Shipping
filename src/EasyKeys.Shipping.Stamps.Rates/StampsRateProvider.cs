using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Services;
using EasyKeys.Shipping.Stamps.Rates.Extensions;
using EasyKeys.Shipping.Stamps.Rates.Models;

using Microsoft.Extensions.Logging;

using StampsClient.v111;

namespace EasyKeys.Shipping.Stamps.Rates;

public class StampsRateProvider : IStampsRateProvider
{
    private readonly IStampsClientService _stampsClientService;
    private readonly ILogger<StampsRateProvider> _logger;

    public StampsRateProvider(
        IStampsClientService stampsClientService,
        ILogger<StampsRateProvider> logger)
    {
        _stampsClientService = stampsClientService ?? throw new ArgumentNullException(nameof(stampsClientService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Shipment> GetRatesAsync(
        Shipment shipment,
        RateOptions rateOptions,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var rateRequest = new RateV40().MapToRate(shipment, rateOptions);

            var rates = await GetRatesAsync(rateRequest, shipment, rateOptions, cancellationToken);

            var packageType = StampsPackageType.FromName(shipment.Options.PackagingType);

            foreach (var rate in rates)
            {
                var rateReturned = new Rate(
                                    rate.ServiceType.ToString(),
                                    rate.ServiceDescription,
                                    rate.PackageType.ToString(),
                                    rate.Amount,
                                    rate.DeliveryDate)
                {
                    TotalCharges2 = rate.Amount
                };

                shipment.Rates.Add(rateReturned);

                _logger.LogDebug($"{rate.ServiceType} - {rate.ServiceDescription} => Packaging: {rate.PackageType} => Amount: {rate.Amount}  => Delivery Days : {rate.DeliverDays}");
            }
        }
        catch (Exception ex)
        {
            var error = ex?.InnerException?.Message ?? ex?.Message ?? string.Empty;
            _logger.LogError("{name} : {message}", nameof(StampsRateProvider), error);
            shipment.InternalErrors.Add(error);
        }

        return shipment;
    }

    private async Task<List<RateV40>> GetRatesAsync(
        RateV40 rate,
        Shipment shipment,
        RateOptions rateOptions,
        CancellationToken cancellationToken)
    {
        var request = new GetRatesRequest()
        {
            Rate = rate,
            Carrier = rateOptions.Carrier.Value switch
            {
                (int)Carrier.USPS => Carrier.USPS,
                (int)Carrier.UPS => Carrier.UPS,
                (int)Carrier.DHLExpress => Carrier.DHLExpress,
                (int)Carrier.FedEx => Carrier.FedEx,
                _ => Carrier.USPS
            }
        };

        var response = await _stampsClientService.GetRatesAsync(request, cancellationToken);

        response = ApplyAddOns(response, rateOptions, shipment);

        return response.Rates.ToList();
    }

    private GetRatesResponse ApplyAddOns(GetRatesResponse response, RateOptions rateDetails, Shipment shipment)
    {
        foreach (var rate in response.Rates)
        {
            var addOns = new List<AddOnV17>();

            if (rate.RequiresAllOf != null)
            {
                addOns.AddRange(AssignRequiredAddOnTypes(rate.RequiresAllOf));

                rate.Amount += addOns.Select(x => x.Amount).Sum();
            }

            if (shipment.Packages.Any(x => x.InsuredValue > 0.0m))
            {
                var insuranceCharge = rate.AddOns.FirstOrDefault(x => x.AddOnType.Equals(AddOnTypeV17.PGAINS))?.Amount;

                rate.Amount += insuranceCharge ?? 0.0m;
            }

            if (shipment.DestinationAddress.IsUnitedStatesAddress())
            {
                // can only chose on or the other unless addon "Registered Mail" is added.
                if (shipment.Packages.Any(x => x.SignatureRequiredOnDelivery))
                {
                    // see href="https://faq.usps.com/s/article/How-is-Signature-Confirmation-and-Signature-Confirmation-Restricted-Delivery-Used"
                    if (!rateDetails.ServiceType.Description.Contains("Express"))
                    {
                        addOns.Add(new AddOnV17()
                        {
                            AddOnDescription = "Signature Confirmation",
                            AddOnType = AddOnTypeV17.USASC
                        });

                        rate.Amount += rate.AddOns.FirstOrDefault(x => x.AddOnType == AddOnTypeV17.USASC)?.Amount ?? 0m;
                    }
                    else
                    {
                        addOns.Add(new AddOnV17()
                        {
                            AddOnDescription = "Delivery Confirmation",
                            AddOnType = AddOnTypeV17.USADC
                        });
                        rate.Amount += rate.AddOns.FirstOrDefault(x => x.AddOnType == AddOnTypeV17.USASC)?.Amount ?? 0m;
                    }
                }
            }

            rate.AddOns = addOns.ToArray();
        }

        return response;
    }

    /// <summary>
    /// Must choose exactly one add-on from each set of add-ons listed in the
    /// requiresallof element in order to form a valid rate to be passed to CreateIndicium.
    /// The integration may use this hint in preparing a user interface with pre-validation for its users.
    /// <see href="file:///C:/Users/ucren/source/repos/EasyKeys.Shipping/src/EasyKeys.Shipping.Stamps.Abstractions/wsdls/SWS%20-%20Developer%20Guide%20v1.0.pdf">Documentta</see>.
    /// </summary>
    /// <param name="ArrayOfArrayOfTypes"></param>
    private List<AddOnV17> AssignRequiredAddOnTypes(AddOnTypeV17[][] ArrayOfArrayOfTypes)
    {
        var addOns = new List<AddOnV17>();
        for (var i = 0; i < ArrayOfArrayOfTypes.Length; i++)
        {
            addOns.Add(new AddOnV17()
            {
                AddOnType = ArrayOfArrayOfTypes[i].FirstOrDefault()
            });
        }

        return addOns;
    }
}
