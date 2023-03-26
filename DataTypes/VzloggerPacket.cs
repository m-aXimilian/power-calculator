namespace DataTypes.Data.Vzlogger.Packet
{
    public class Data : IData
    {
        public string uuid { get; set; }
        public long last { get; set; }
        public int interval { get; set; }
        public string protocol { get; set; }
        public List<List<double>> tuples { get; set; }
    }

    public class VzloggerPacket : IPacket
    {
        public string version { get; set; }
        public string generator { get; set; }

        public List<Data> data { get; set; }
    }
}