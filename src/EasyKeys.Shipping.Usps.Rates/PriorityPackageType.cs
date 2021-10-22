using EasyKeys.Shipping.Abstractions.Models;

namespace EasyKeys.Shipping.Usps.Rates;

/// <summary>
/// These types are used to configure <see cref="ShipmentOptions"/> for domestic priority mail packages.
/// </summary>
public enum PriorityPackageType
{
    VARIABLE,
    FLAT_RATE_ENVELOPE,
    PADDED_FLAT_RATE_ENVELOPE,
    LEGAL_FLAT_RATE_ENVELOPE,
    SM_FLAT_RATE_ENVELOPE,
    WINDOW_FLAT_RATE_ENVELOPE,
    GIFT_CARD_FLAT_RATE_ENVELOPE,
    SM_FLAT_RATE_BOX,
    MD_FLAT_RATE_BOX,
    LG_FLAT_RATE_BOX,
    REGIONALRATEBOXA,
    REGIONALRATEBOXB,
    CUBIC_PARCELS,
    CUBIC_SOFT_PACK,
}
