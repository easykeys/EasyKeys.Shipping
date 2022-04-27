using EasyKeys.Shipping.Abstractions.Models;

namespace EasyKeysShipping.UnitTest;

public class DimensionsTests
{
    [Fact]
    public void Test_Dimensions_Successfully()
    {
        // first class package domestic and international and int box are the same size
        var package = new Dimensions(8m, 4m, 0.25m);

        Assert.Equal(17, package.Measurement);
        Assert.Equal(9, package.Girth);

        var fpackage = new Dimensions(11m, 6m, 0.75m);

        // M: 25 G: 14
        Assert.Equal(25, fpackage.Measurement);
        Assert.Equal(14, fpackage.Girth);

        var small = new Dimensions(8m, 5m, 1m);

        // M: 20 G: 12
        Assert.Equal(20, small.Measurement);
        Assert.Equal(12, small.Girth);

        var medium = new Dimensions(11m, 8m, 5m);

        // M: 37 G: 26
        Assert.Equal(37, medium.Measurement);
        Assert.Equal(26, medium.Girth);

        var fedExEnvelope = new Dimensions(9m, 12m, 0.25m);

        // M: 34 G: 25
        Assert.Equal(34, fedExEnvelope.Measurement);
        Assert.Equal(25, fedExEnvelope.Girth);

        var fedExPak = new Dimensions(12m, 15m, 1.5m);

        // M: 45 G: 33
        Assert.Equal(45, fedExPak.Measurement);
        Assert.Equal(33, fedExPak.Girth);
    }
}
