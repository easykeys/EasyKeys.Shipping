using EasyKeys.Shipping.Abstractions.Models;

namespace EasyKeysShipping.UnitTest;

public class PackageWeightTests
{
    [Theory]
    [InlineData(1, 16)]
    [InlineData(2, 32)]
    public void Test_Weight_In_Pounds(int pounds, decimal ounces)
    {
        var package = new PackageWeight(pounds, isOunce: false);
        Assert.Equal(ounces, package.InOunces);
    }

    [Theory]
    [InlineData(16, 1)]
    [InlineData(32, 2)]
    [InlineData(15.99, 0.999375)]
    public void Test_Weight_In_Ounces(decimal ounces, decimal pounds)
    {
        var package = new PackageWeight(ounces, isOunce: true);
        Assert.Equal(pounds, package.InPounds);
    }
}
