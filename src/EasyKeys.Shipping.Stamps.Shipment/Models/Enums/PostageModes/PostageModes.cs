using Ardalis.SmartEnum;

using StampsClient.v111;

namespace EasyKeys.Shipping.Stamps.Shipment.Models.Enums.PostageModes
{
    public abstract class PostageModes : SmartEnum<PostageModes>
    {
        public static readonly PostageModes Normal = new NormalPostage();

        public static readonly PostageModes NoPostage = new NoPostage();

        public PostageModes(string name, int value, PostageMode mode) : base(name, value)
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
