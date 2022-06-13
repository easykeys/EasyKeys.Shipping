using Ardalis.SmartEnum;

using ShipClient.v25;

namespace EasyKeys.Shipping.FedEx.Shipment.Models;

public abstract class FedExNotificationEventType : SmartEnum<FedExNotificationEventType>
{
    public static readonly FedExNotificationEventType OnShipment = new OnShipmentType();
    public static readonly FedExNotificationEventType OnDelivery = new OnDeliveryType();
    public static readonly FedExNotificationEventType OnEstimatedDelivery = new OnEstimatedDeliveryType();
    public static readonly FedExNotificationEventType OnException = new OnExceptionType();
    public static readonly FedExNotificationEventType OnPickupDriverArrived = new OnPickupDriverArrivedType();
    public static readonly FedExNotificationEventType OnPickupDriverAssigned = new OnPickupDriverAssignedType();
    public static readonly FedExNotificationEventType OnPickupDriverDeparted = new OnPickupDriverDepartedType();
    public static readonly FedExNotificationEventType OnPickupDriverEnRoute = new OnPickupDriverEnRouteType();
    public static readonly FedExNotificationEventType OnTender = new OnTenderType();

    protected FedExNotificationEventType(string name, int value) : base(name, value)
    {
    }

    private class OnShipmentType : FedExNotificationEventType
    {
        public OnShipmentType() : base(nameof(NotificationEventType.ON_SHIPMENT), (int)NotificationEventType.ON_SHIPMENT)
        {
        }
    }

    private class OnDeliveryType : FedExNotificationEventType
    {
        public OnDeliveryType() : base(nameof(NotificationEventType.ON_DELIVERY), (int)NotificationEventType.ON_DELIVERY)
        {
        }
    }

    private class OnEstimatedDeliveryType : FedExNotificationEventType
    {
        public OnEstimatedDeliveryType() : base(nameof(NotificationEventType.ON_ESTIMATED_DELIVERY), (int)NotificationEventType.ON_ESTIMATED_DELIVERY)
        {
        }
    }

    private class OnExceptionType : FedExNotificationEventType
    {
        public OnExceptionType() : base(nameof(NotificationEventType.ON_EXCEPTION), (int)NotificationEventType.ON_EXCEPTION)
        {
        }
    }

    private class OnPickupDriverArrivedType : FedExNotificationEventType
    {
        public OnPickupDriverArrivedType() : base(nameof(NotificationEventType.ON_PICKUP_DRIVER_ARRIVED), (int)NotificationEventType.ON_PICKUP_DRIVER_ARRIVED)
        {
        }
    }

    private class OnPickupDriverAssignedType : FedExNotificationEventType
    {
        public OnPickupDriverAssignedType() : base(nameof(NotificationEventType.ON_PICKUP_DRIVER_ASSIGNED), (int)NotificationEventType.ON_PICKUP_DRIVER_ASSIGNED)
        {
        }
    }

    private class OnPickupDriverDepartedType : FedExNotificationEventType
    {
        public OnPickupDriverDepartedType() : base(nameof(NotificationEventType.ON_PICKUP_DRIVER_DEPARTED), (int)NotificationEventType.ON_PICKUP_DRIVER_DEPARTED)
        {
        }
    }

    private class OnPickupDriverEnRouteType : FedExNotificationEventType
    {
        public OnPickupDriverEnRouteType() : base(nameof(NotificationEventType.ON_PICKUP_DRIVER_EN_ROUTE), (int)NotificationEventType.ON_PICKUP_DRIVER_EN_ROUTE)
        {
        }
    }

    private class OnTenderType : FedExNotificationEventType
    {
        public OnTenderType() : base(nameof(NotificationEventType.ON_TENDER), (int)NotificationEventType.ON_TENDER)
        {
        }
    }
}
