namespace EasyKeys.Shipping.PostalAddress;

public interface IAddressParser
{
    /// <summary>
    /// Attempts to parse US based address.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    bool TryParseAddress(string input, out AddressParseResult? result);
}
