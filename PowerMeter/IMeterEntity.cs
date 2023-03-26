namespace Domain.PowerMeter
{
    public enum EnergyDirection { CONSUMPTION, PRODUCTION, NONE }
    public interface IMeterEntity
    {
        public event EventHandler NewMeterCollectionAvailable;

        public IDictionary<string, string> ChannelCollecton { get; }

        public EnergyDirection ChannelType { get; }

        public List<IMeterCollection> MeterReadings { get; set; }
    }
}