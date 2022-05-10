using Ardalis.SmartEnum;

namespace EasyKeys.Shipping.Stamps.Shipment.Models
{
    public abstract class PostageMode : SmartEnum<PostageMode>
    {
        public static readonly PostageModes Normal = new NormalPostage();

        public static readonly PostageModes NoPostage = new NoPostage();

        public PostageMode(string name, int value, PostageMode mode) : base(name, value)
        {
            Type = mode;
        }

        public PostageMode Type
        {
            get;
            private set;
        }
    }
}
