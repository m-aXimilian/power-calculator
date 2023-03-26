using DataTypes.Data;

namespace Domain.PowerMeter
{
    public class PowerMeterCollection : IMeterCollection
    {
        public PowerMeterCollection(IEnumerable<IAbstractData> dataT1)
        {
            this.Data = dataT1 ?? throw new ArgumentNullException(nameof(dataT1));
            this.Id = Guid.NewGuid();
            this.Timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
        }

        public IEnumerable<IAbstractData> Data { get; }

        public Guid Id { get; }

        public long Timestamp { get; }
    }
}