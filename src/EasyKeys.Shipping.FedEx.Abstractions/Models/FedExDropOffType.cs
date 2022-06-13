using Ardalis.SmartEnum;

using RateClient.v28;

namespace EasyKeys.Shipping.FedEx.Abstractions.Models;

public abstract class FedExDropOffType : SmartEnum<FedExDropOffType>
{
    public static readonly FedExDropOffType BusinessServiceCenter = new BusinessServiceCenterType();
    public static readonly FedExDropOffType RegularPickup = new RegularPickupType();
    public static readonly FedExDropOffType DropBox = new DropBoxType();
    public static readonly FedExDropOffType RequestCourier = new RequestCourierType();
    public static readonly FedExDropOffType Station = new StationType();

    protected FedExDropOffType(string name, int value) : base(name, value)
    {
    }

    private class BusinessServiceCenterType : FedExDropOffType
    {
        public BusinessServiceCenterType() : base(DropoffType.BUSINESS_SERVICE_CENTER.ToString(), (int)DropoffType.BUSINESS_SERVICE_CENTER)
        {
        }
    }

    private class RegularPickupType : FedExDropOffType
    {
        public RegularPickupType() : base(DropoffType.REGULAR_PICKUP.ToString(), (int)DropoffType.REGULAR_PICKUP)
        {
        }
    }

    private class DropBoxType : FedExDropOffType
    {
        public DropBoxType() : base(DropoffType.DROP_BOX.ToString(), (int)DropoffType.DROP_BOX)
        {
        }
    }

    private class RequestCourierType : FedExDropOffType
    {
        public RequestCourierType() : base(DropoffType.REQUEST_COURIER.ToString(), (int)DropoffType.REQUEST_COURIER)
        {
        }
    }

    private class StationType : FedExDropOffType
    {
        public StationType() : base(DropoffType.STATION.ToString(), (int)DropoffType.STATION)
        {
        }
    }
}
