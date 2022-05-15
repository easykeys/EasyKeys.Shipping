using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Models;

using StampsClient.v111;

namespace EasyKeys.Shipping.Stamps.Abstractions.Services.Impl;

public class RatesService : IRatesService
{
    private readonly IStampsClientService _stampsClient;

    public RatesService(IStampsClientService stampsClientService)
    {
        _stampsClient = stampsClientService;
    }

    public async Task<List<RateV40>> GetRatesResponseAsync(Shipment shipment, RateRequestDetails rateDetails, CancellationToken cancellationToken)
    {
        {
            var stampsClient = _stampsClient.CreateClient();

            var request = new GetRatesRequest()
            {
                Item = await _stampsClient.GetTokenAsync(cancellationToken),

                Rate = new RateV40()
                {
                    From = new StampsClient.v111.Address()
                    {
                        FullName = shipment.SenderInfo.FullName,

                        FirstName = shipment.SenderInfo.FirstName,

                        LastName = shipment.SenderInfo.LastName,

                        Address1 = shipment.OriginAddress.StreetLine,

                        City = shipment.OriginAddress.City,

                        State = shipment.OriginAddress.StateOrProvince,

                        Country = shipment.OriginAddress.CountryCode,

                        ZIPCode = shipment.OriginAddress.PostalCode,

                        PhoneNumber = shipment.SenderInfo.PhoneNumber,

                        EmailAddress = shipment.RecipientInfo.Email
                    },

                    To = new StampsClient.v111.Address()
                    {
                        FullName = shipment.RecipientInfo.FullName,

                        FirstName = shipment.RecipientInfo.FirstName,

                        LastName = shipment.RecipientInfo.LastName,

                        Address1 = shipment.DestinationAddress.StreetLine,

                        City = shipment.DestinationAddress.City,

                        Province = shipment.DestinationAddress.IsUnitedStatesAddress() ? null : shipment.DestinationAddress.StateOrProvince,

                        State = shipment.DestinationAddress.IsUnitedStatesAddress() ? shipment.DestinationAddress.StateOrProvince : null,

                        Country = shipment.DestinationAddress.CountryCode,

                        PostalCode = shipment.DestinationAddress.IsUnitedStatesAddress() ? null : shipment.DestinationAddress.PostalCode,

                        ZIPCode = shipment.DestinationAddress.IsUnitedStatesAddress() ? shipment.DestinationAddress.PostalCode : null,

                        PhoneNumber = shipment.RecipientInfo.PhoneNumber,

                        EmailAddress = shipment.RecipientInfo.Email
                    },

                    Amount = rateDetails.Amount,

                    MaxAmount = rateDetails.MaxAmount,

                    ServiceType = rateDetails.ServiceType.Value switch
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

                    ServiceDescription = rateDetails.ServiceType.Description,

                    DeliverDays = string.Empty,

                    ShipDate = shipment.Options.ShippingDate,

                    InsuredValue = shipment.Packages.Sum(x => x.InsuredValue),

                    RegisteredValue = rateDetails.RegisteredValue,

                    CODValue = rateDetails.CODValue,

                    DeclaredValue = rateDetails.DeclaredValue,

                    NonMachinable = rateDetails.NonMachinable,

                    RectangularShaped = rateDetails.RectangularShaped,

                    Prohibitions = rateDetails.Prohibitions,

                    Restrictions = rateDetails.Restrictions,

                    Observations = rateDetails.Observations,

                    Regulations = rateDetails.Regulations,

                    GEMNotes = rateDetails.GEMNotes,

                    MaxDimensions = rateDetails.MaxDimensions,

                    DimWeighting = rateDetails.DimWeighting,

                    EffectiveWeightInOunces = 0,

                    CubicPricing = rateDetails.CubicPricing,

                    ContentType = rateDetails.ContentType.Value switch
                    {
                        (int)ContentTypeV2.CommercialSample => ContentTypeV2.CommercialSample,
                        (int)ContentTypeV2.DangerousGoods => ContentTypeV2.DangerousGoods,
                        (int)ContentTypeV2.Document => ContentTypeV2.Document,
                        (int)ContentTypeV2.Gift => ContentTypeV2.Gift,
                        (int)ContentTypeV2.HumanitarianDonation => ContentTypeV2.HumanitarianDonation,
                        (int)ContentTypeV2.Merchandise => ContentTypeV2.Merchandise,
                        (int)ContentTypeV2.ReturnedGoods => ContentTypeV2.ReturnedGoods,
                        _ => ContentTypeV2.Other
                    },

                    ContentTypeSpecified = rateDetails.ContentTypeSpecified,

                    // dont know what this is
                    EntryFacility = EntryFacilityV1.Unknown,

                    // dont know what this is
                    SortType = SortTypeV1.Unknown,
                },
                Carrier = rateDetails.Carrier.Value switch
                {
                    (int)Carrier.USPS => Carrier.USPS,
                    (int)Carrier.UPS => Carrier.UPS,
                    (int)Carrier.DHLExpress => Carrier.DHLExpress,
                    (int)Carrier.FedEx => Carrier.FedEx,
                    _ => Carrier.USPS
                }
            };

            request = ApplyPackageDetails(request, shipment);

            var response = await stampsClient.GetRatesAsync(request);

            response = ApplyAddOns(response, rateDetails, shipment);

            return response.Rates.ToList();
        }
    }

    private GetRatesResponse ApplyAddOns(GetRatesResponse request, RateRequestDetails rateDetails, Shipment shipment)
    {
        foreach (var rate in request.Rates)
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
                        addOns.Add(new AddOnV17() { AddOnDescription = "Signature Confirmation", AddOnType = AddOnTypeV17.USASC });
                        rate.Amount += rate
                                        .AddOns.FirstOrDefault(x => x.AddOnType == AddOnTypeV17.USASC)
                                        ?.Amount ?? 0m;
                    }
                    else
                    {
                        addOns.Add(new AddOnV17() { AddOnDescription = "Delivery Confirmation", AddOnType = AddOnTypeV17.USADC });
                        rate.Amount += rate
                                        .AddOns.FirstOrDefault(x => x.AddOnType == AddOnTypeV17.USASC)
                                        ?.Amount ?? 0m;
                    }
                }
            }

            rate.AddOns = addOns.ToArray();
        }

        return request;
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
            addOns.Add(new AddOnV17() { AddOnType = ArrayOfArrayOfTypes[i].FirstOrDefault() });
        }

        return addOns;
    }

    private GetRatesRequest ApplyPackageDetails(GetRatesRequest request, Shipment shipment)
    {
        request.Rate.WeightLb = (double)shipment.Packages.FirstOrDefault().Weight;

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

        return request;
    }
}
