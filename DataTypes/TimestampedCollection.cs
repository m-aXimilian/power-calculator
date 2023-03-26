namespace DataTypes.Data
{
    public class TimestampedCollection<T>
    {
        public TimestampedCollection(T data)
        {
            this.Data = data;
            this.Timestamp = (ulong)new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
        }
        public ulong Timestamp { get; }

        public T Data { get; }
    }
}
