using EasyKeys.Shipping.Abstractions.Models;

namespace EasyKeys.Shipping.Abstractions;

public static class AddressExtensions
{
    /// <summary>
    /// Get street lines array.
    /// </summary>
    /// <param name="address"></param>
    /// <returns></returns>
    public static string[] GetStreetLines(this Address address)
    {
        var streetLines = new List<string>
            {
                address.StreetLine.Trim(),
                address?.StreetLine2?.Trim() ?? string.Empty,
            };
        streetLines = streetLines.Where(l => !string.IsNullOrEmpty(l)).ToList();
        return streetLines.Any() ? streetLines.ToArray() : new string[] { string.Empty };
    }
}
