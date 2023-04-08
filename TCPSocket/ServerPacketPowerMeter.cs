using DataTypes.Data;

namespace Domain.PowerMeter
{
    public class ServerPacketPowerMeter : IReadOnlyMeanPowerCollection<TimestampedCollection<double>>
    {
        public ServerPacketPowerMeter(
            TimestampedCollection<double> currentMeanPower,
            TimestampedCollection<double> recentLivePower,
            double averagePowerTotalTimeDelta,
            TimestampedCollection<double> currentPowerDrift
        )
        {
            this.CurrentMeanPower = currentMeanPower;
            this.RecentLivePower = recentLivePower;
            this.AveragePowerTotalTimeDelta = averagePowerTotalTimeDelta;
            this.CurrentPowerDrift = currentPowerDrift;
        }
        public TimestampedCollection<double> CurrentMeanPower { get; }

        public TimestampedCollection<double> RecentLivePower { get; }

        public double AveragePowerTotalTimeDelta { get; }

        public TimestampedCollection<double> CurrentPowerDrift { get; }
    }
}