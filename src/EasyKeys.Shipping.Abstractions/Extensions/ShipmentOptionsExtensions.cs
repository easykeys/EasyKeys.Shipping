using EasyKeys.Shipping.Abstractions.Models;

namespace EasyKeys.Shipping.Abstractions.Extensions;

public static class ShipmentOptionsExtensions
{
    public static string GetCurrencyCode(this ShipmentOptions options)
    {
        return !string.IsNullOrEmpty(options?.PreferredCurrencyCode)
            ? options?.PreferredCurrencyCode ?? ShipmentOptions.DefaultCurrencyCode
            : ShipmentOptions.DefaultCurrencyCode;
    }
}
