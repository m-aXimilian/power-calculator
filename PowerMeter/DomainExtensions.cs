using DataTypes.Data;

namespace Domain.PowerMeter
{
    public static class DomainExtensions
    {
        public static ServerPacketPowerMeter WrapServerPacket(
            this IReadOnlyMeanPowerCollection<TimestampedCollection<double>> meanPowerCalculator)
        {
            var packet = new ServerPacketPowerMeter(
                meanPowerCalculator.CurrentMeanPower,
                meanPowerCalculator.RecentLivePower,
                meanPowerCalculator.AveragePowerTotalTimeDelta,
                meanPowerCalculator.CurrentPowerDrift
            );

            return packet;
        }
    }
}