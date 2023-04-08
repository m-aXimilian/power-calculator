namespace DataTypes.Data
{
    public interface IReadOnlyMeanPowerCollection<T>
    {
        public T CurrentMeanPower { get; }

        public T RecentLivePower { get; }

        public double AveragePowerTotalTimeDelta { get; }

        public T CurrentPowerDrift { get; }
    }
}