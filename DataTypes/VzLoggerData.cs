namespace DataTypes.Data.Vzlogger
{
    public class VzloggerData : IAbstractData
    {
        private readonly IPacket packet;

        public VzloggerData(IPacket packet)
        {
            this.packet = packet ?? throw new ArgumentNullException(nameof(packet));
            this.DataTuple = ((ulong)packet.data[0].tuples[0][0], (double)packet.data[0].tuples[0][1]);
            this.Uuid = packet.data[0].uuid;
        }

        public (ulong, double) DataTuple { get; }

        public string Uuid { get; }
    }
}