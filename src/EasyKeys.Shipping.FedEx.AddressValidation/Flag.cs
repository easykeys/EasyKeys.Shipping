using AddressValidationClient.v4;

namespace EasyKeys.Shipping.FedEx.AddressValidation;

public class Flag
{
    public Flag(Type type, params OperationalAddressStateType[] stateTypes)
    {
        Type = type;
        StateType.AddRange(stateTypes);
    }

    public Type? Type { get; set; }

    public List<OperationalAddressStateType> StateType { get; } = new List<OperationalAddressStateType>();
}
