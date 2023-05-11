namespace EasyKeys.Shipping.Abstractions.Models;

public struct PackageWeight
{
    private decimal _ounces;
    private decimal _pounds;

    public PackageWeight(decimal weight, bool isOunce)
    {
        if (isOunce)
        {
            SetInOunces(weight);
        }
        else
        {
            SetWeightInLbs(weight);
        }
    }

    public decimal InPounds => _pounds;

    public decimal InOunces => _ounces;

    public PoundsAndOunces PoundsAndOunces
    {
        get
        {
            var result = default(PoundsAndOunces);

            if (_pounds <= 0)
            {
                return result;
            }

            result.Pounds = (int)Math.Truncate(_pounds);
            var decimalPart = (_pounds - result.Pounds) * 16;

            result.Ounces = (int)Math.Ceiling(decimalPart);

            return result;
        }
    }

    private void SetWeightInLbs(decimal pounds)
    {
        _pounds = pounds;
        _ounces = pounds * 16;
    }

    private void SetInOunces(decimal ounces)
    {
        _ounces = ounces;
        _pounds = ounces / 16;
    }
}
