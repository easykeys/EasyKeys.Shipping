namespace EasyKeys.Shipping.PostalAddress
{
    public interface IAddressParser
    {
        bool TryParseAddress(string input, out AddressParseResult? result);
    }
}
