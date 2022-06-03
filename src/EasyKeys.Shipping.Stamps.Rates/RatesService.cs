using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Rates.Extensions;
using EasyKeys.Shipping.Stamps.Rates.Models;

using StampsClient.v111;

namespace EasyKeys.Shipping.Stamps.Abstractions.Services.Impl;

public class RatesService : IRatesService
{
    private readonly IStampsClientService _stampsClient;

    public RatesService(IStampsClientService stampsClientService)
    {
        _stampsClient = stampsClientService ?? throw new ArgumentNullException(nameof(stampsClientService));
    }

    public async Task<List<RateV40>> GetRatesResponseAsync(
        Shipment shipment,
        RateOptions rateOptions,
        CancellationToken cancellationToken)
    {
        var rate = new RateV40()
        {
            From = shipment.OriginAddress.Map(rateOptions.Sender),

            To = shipment.DestinationAddress.Map(rateOptions.Recipient),

            Amount = rateOptions.Amount,

            MaxAmount = rateOptions.MaxAmount,

            ServiceType = rateOptions.ServiceType.Value switch
            {
                (int)ServiceType.USPS => ServiceType.USPS,
                (int)ServiceType.USFC => ServiceType.USFC,
                (int)ServiceType.USMM => ServiceType.USMM,
                (int)ServiceType.USPM => ServiceType.USPM,
                (int)ServiceType.USXM => ServiceType.USXM,
                (int)ServiceType.USEMI => ServiceType.USEMI,
                (int)ServiceType.USFCI => ServiceType.USFCI,
                (int)ServiceType.USRETURN => ServiceType.USRETURN,
                (int)ServiceType.USLM => ServiceType.USLM,
                (int)ServiceType.USPMI => ServiceType.USPMI,
                (int)ServiceType.Unknown => ServiceType.Unknown,
                _ => ServiceType.Unknown
            },

            ServiceDescription = rateOptions.ServiceType.Description,

            DeliverDays = string.Empty,

            ShipDate = shipment.Options.ShippingDate,

            InsuredValue = shipment.Packages.Sum(x => x.InsuredValue),

            RegisteredValue = shipment.Packages.FirstOrDefault().InsuredValue,

            CODValue = rateOptions.CODValue,

            DeclaredValue = shipment.Packages.FirstOrDefault().InsuredValue,

            NonMachinable = rateOptions.NonMachinable,

            RectangularShaped = rateOptions.RectangularShaped,

            MaxDimensions = rateOptions.MaxDimensions,

            DimWeighting = rateOptions.DimWeighting,

            EffectiveWeightInOunces = 0,

            CubicPricing = rateOptions.CubicPricing,

            ContentTypeSpecified = false,
        };

        return await GetRatesAsync(rate, shipment, rateOptions, cancellationToken);
    }

    public async Task<List<RateV40>> GetInternationalRatesAsync(
        Shipment shipment,
        RateInternationalOptions rateOptions,
        CancellationToken cancellationToken)
    {
        var rate = new RateV40()
        {
            From = shipment.OriginAddress.Map(rateOptions.Sender),

            To = shipment.DestinationAddress.Map(rateOptions.Recipient),

            Amount = rateOptions.Amount,

            MaxAmount = rateOptions.MaxAmount,

            ServiceType = rateOptions.ServiceType.Value switch
            {
                (int)ServiceType.USPS => ServiceType.USPS,
                (int)ServiceType.USFC => ServiceType.USFC,
                (int)ServiceType.USMM => ServiceType.USMM,
                (int)ServiceType.USPM => ServiceType.USPM,
                (int)ServiceType.USXM => ServiceType.USXM,
                (int)ServiceType.USEMI => ServiceType.USEMI,
                (int)ServiceType.USFCI => ServiceType.USFCI,
                (int)ServiceType.USRETURN => ServiceType.USRETURN,
                (int)ServiceType.USLM => ServiceType.USLM,
                (int)ServiceType.USPMI => ServiceType.USPMI,
                (int)ServiceType.Unknown => ServiceType.Unknown,
                _ => ServiceType.Unknown
            },

            ServiceDescription = rateOptions.ServiceType.Description,

            DeliverDays = string.Empty,

            ShipDate = shipment.Options.ShippingDate,

            InsuredValue = shipment.Packages.Sum(x => x.InsuredValue),

            RegisteredValue = shipment.Packages.FirstOrDefault().InsuredValue,

            CODValue = rateOptions.CODValue,

            DeclaredValue = shipment.Packages.FirstOrDefault().InsuredValue,

            NonMachinable = rateOptions.NonMachinable,

            RectangularShaped = rateOptions.RectangularShaped,

            Prohibitions = rateOptions.Prohibitions,

            Restrictions = rateOptions.Restrictions,

            Observations = rateOptions.Observations,

            Regulations = rateOptions.Regulations,

            GEMNotes = rateOptions.GEMNotes,

            MaxDimensions = rateOptions.MaxDimensions,

            DimWeighting = rateOptions.DimWeighting,

            EffectiveWeightInOunces = 0,

            CubicPricing = rateOptions.CubicPricing,

            ContentTypeSpecified = false,
        };

        return await GetRatesAsync(rate, shipment, rateOptions, cancellationToken);
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

        request = ApplyPackageDetails(request, shipment);

        var response = await _stampsClient.GetRatesAsync(request, cancellationToken);

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
                        addOns.Add(new AddOnV17() { AddOnDescription = "Delivery Confirmation", AddOnType = AddOnTypeV17.USADC });
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

    private GetRatesRequest ApplyPackageDetails(GetRatesRequest request, Shipment shipment)
    {
        request.Rate.WeightLb = shipment.Packages.Sum(x => x.PoundsAndOunces.Pounds);

        request.Rate.WeightOz = 0.0;

        request.Rate.PackageType = PackageType.FromName(shipment.Options.PackagingType).Value switch
        {
            (int)PackageTypeV11.Pak => PackageTypeV11.Pak,
            (int)PackageTypeV11.Package => PackageTypeV11.Package,
            (int)PackageTypeV11.OversizedPackage => PackageTypeV11.OversizedPackage,
            (int)PackageTypeV11.LargePackage => PackageTypeV11.LargePackage,
            (int)PackageTypeV11.Postcard => PackageTypeV11.Postcard,
            (int)PackageTypeV11.Documents => PackageTypeV11.Documents,
            (int)PackageTypeV11.ThickEnvelope => PackageTypeV11.ThickEnvelope,
            (int)PackageTypeV11.Envelope => PackageTypeV11.Envelope,
            (int)PackageTypeV11.ExpressEnvelope => PackageTypeV11.ExpressEnvelope,
            (int)PackageTypeV11.FlatRateEnvelope => PackageTypeV11.FlatRateEnvelope,
            (int)PackageTypeV11.LegalFlatRateEnvelope => PackageTypeV11.LegalFlatRateEnvelope,
            (int)PackageTypeV11.Letter => PackageTypeV11.Letter,
            (int)PackageTypeV11.LargeEnvelopeorFlat => PackageTypeV11.LargeEnvelopeorFlat,
            (int)PackageTypeV11.SmallFlatRateBox => PackageTypeV11.SmallFlatRateBox,
            (int)PackageTypeV11.FlatRateBox => PackageTypeV11.FlatRateBox,
            (int)PackageTypeV11.LargeFlatRateBox => PackageTypeV11.LargeFlatRateBox,
            (int)PackageTypeV11.FlatRatePaddedEnvelope => PackageTypeV11.FlatRatePaddedEnvelope,
            (int)PackageTypeV11.RegionalRateBoxA => PackageTypeV11.RegionalRateBoxA,
            (int)PackageTypeV11.RegionalRateBoxB => PackageTypeV11.RegionalRateBoxB,
            (int)PackageTypeV11.RegionalRateBoxC => PackageTypeV11.RegionalRateBoxC,
            _ => PackageTypeV11.Package
        };

        request.Rate.Length = (double)shipment.Packages.FirstOrDefault().Dimensions.Length;

        request.Rate.Width = (double)shipment.Packages.FirstOrDefault().Dimensions.Width;

        request.Rate.Height = (double)shipment.Packages.FirstOrDefault().Dimensions.Height;

        // priority express & priority express international service types do not except any box package types
        if (request.Rate.ServiceType == ServiceType.USXM || request.Rate.ServiceType == ServiceType.USEMI)
        {
            request.Rate.PackageType = request.Rate.PackageType.ToString().Contains("Box") ? PackageTypeV11.Package : request.Rate.PackageType;
        }

        return request;
    }
}
