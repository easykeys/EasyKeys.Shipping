using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.DHL.Abstractions;

using EasyKeysShipping.FuncTest.TestHelpers;

using Microsoft.Extensions.DependencyInjection;

namespace EasyKeysShipping.FuncTest.DHL;

public class DHLAddressValidationProviderTests
{
    private readonly ITestOutputHelper _output;
    private readonly IEnumerable<IDHLExpressAddressValidationProvider> _validators;

    public DHLAddressValidationProviderTests(ITestOutputHelper output)
    {
        _output = output;
        _validators = ServiceProviderInstance.GetDHLServices(output).GetServices<IDHLExpressAddressValidationProvider>();
    }

    [Fact]
    public async Task Validate_International_Address_Successfully()
    {
        var caAddress = new Address("3601 72 Ave Se", "Calgary", "AB", "T2C 2K3", "CA", isResidential: false);
        var chinaAddress = new Address("285 Wang Fu Jing Avenue", "BEIJING", "", "100006", "CN", isResidential: false);

        var requests = new List<ValidateAddress>
        {
            new ValidateAddress(Guid.NewGuid().ToString(), caAddress),
            new ValidateAddress(Guid.NewGuid().ToString(), chinaAddress)
        };

        foreach (var request in requests)
        {
            foreach (var validator in _validators)
            {
                var result = await validator.ValidateAddressAsync(request);
            }
        }
    }
}
