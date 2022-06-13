using Ardalis.SmartEnum;

namespace EasyKeys.Shipping.Stamps.Shipment.Models
{
    public abstract class PostageModeType : SmartEnum<PostageModeType>
    {
        public static readonly PostageModeType Normal = new NormalType();

        public static readonly PostageModeType NoPostage = new NoPostageType();

        public PostageModeType(string name, int value, string description) : base(name, value)
        {
            Description = description;
        }

        public string Description { get; }

        private sealed class NormalType : PostageModeType
        {
            public NormalType() : base(StampsClient.v111.PostageMode.Normal.ToString(), (int)StampsClient.v111.PostageMode.Normal, "Postage Mode Normal")
            {
            }
        }

        private sealed class NoPostageType : PostageModeType
        {
            public NoPostageType() : base(StampsClient.v111.PostageMode.NoPostage.ToString(), (int)StampsClient.v111.PostageMode.NoPostage, "No Postage Mode")
            {
            }
        }
    }
}
