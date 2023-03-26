namespace DataTypes.Data
{
    public interface IPacket
    {
        string version { get; set; }
        string generator { get; set; }

        // Json.Serialize cannot deserialize interface types 
        // therefore concrete type isused here.
        List<DataTypes.Data.Vzlogger.Packet.Data> data { get; set; }
    }
}