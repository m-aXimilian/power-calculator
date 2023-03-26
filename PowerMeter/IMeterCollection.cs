using DataTypes.Data;

namespace Domain.PowerMeter
{
    public interface IMeterCollection
    {
        public IEnumerable<IAbstractData> Data { get; }

        public Guid Id { get; }

        public long Timestamp { get; }
    }
}