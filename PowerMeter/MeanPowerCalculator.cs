using DataTypes.Data;
using Serilog;

namespace Domain.PowerMeter
{
    public class MeanPowerCalculator : IReadOnlyMeanPowerCollection<TimestampedCollection<double>>
    {
        private readonly IMeterEntity meterEntity;

        private event EventHandler RefreshPowerDrift;

        public MeanPowerCalculator(IMeterEntity meterEntity)
        {
            this.meterEntity = meterEntity ?? throw new ArgumentNullException(nameof(meterEntity));
            this.meterEntity.NewMeterCollectionAvailable += this.CalculateMeanPower;
            this.RefreshPowerDrift += this.RefreshPowerDrift_DoWork;
        }

        public TimestampedCollection<double> CurrentMeanPower { get; private set; }

        public TimestampedCollection<double> RecentLivePower { get; private set; }

        public double AveragePowerTotalTimeDelta { get; private set; }

        public TimestampedCollection<double> CurrentPowerDrift { get; private set; }

        private void CalculateMeanPower(object sender, EventArgs e)
        {
            int nElements = this.meterEntity.MeterReadings.Count;
            if (nElements < 2)
            {
                // at least 2 readings needed to compute a difference
                return;
            }

            double sumTemp = 0.0f;
            double recentPower = 0.0f;
            int nEach = this.meterEntity.MeterReadings.First().Data.Count();
            for (int i = 0; i < nElements - 1; i++)
            {
                var tupleT0 = this.meterEntity.MeterReadings[i].Data;
                var tupleT1 = this.meterEntity.MeterReadings[i + 1].Data;
                recentPower = this.TupleCompare(tupleT0, tupleT1);
                sumTemp += recentPower;
            }

            this.RecentLivePower = new(recentPower);
            var currentMean = sumTemp / (nElements - 1);
            var averageTimespan = (this.meterEntity.MeterReadings.Last().Data.First().DataTuple.Item1 -
                                this.meterEntity.MeterReadings.First().Data.First().DataTuple.Item1) / 1_000;

            this.CurrentMeanPower = new(currentMean);
            this.AveragePowerTotalTimeDelta = averageTimespan;

            Log.Debug($"{nameof(this.CalculateMeanPower)}: Current mean for {this.meterEntity.ChannelType}: " +
                            $"{currentMean:0.000}kW in {averageTimespan}s with {nElements} elements of {nEach} each.");

            this.OnRefreshPowerDrift();
        }

        private void OnRefreshPowerDrift()
        {
            var del = RefreshPowerDrift as EventHandler;
            if (del != null)
            {
                del(new Object(), EventArgs.Empty);
            }
        }

        private void RefreshPowerDrift_DoWork(object sender, EventArgs e)
        {
            this.CurrentPowerDrift = new((this.RecentLivePower.Data / this.CurrentMeanPower.Data) - 1);
            Log.Debug($"Current drift {this.CurrentPowerDrift.Data * 100:0.000}%");
        }

        private double TupleCompare(IEnumerable<IAbstractData> firstTuples, IEnumerable<IAbstractData> secondTuples)
        {
            if (firstTuples.Count() != secondTuples.Count())
            {
                Log.Error($"{nameof(this.TupleCompare)}: Sizes of tuples must match!");
                throw new ArgumentException("Sizes of tuple collections must match");
            }

            double tmpSum = 0.0f;
            int length = firstTuples.Count();
            for (int i = 0; i < firstTuples.Count(); i++)
            {
                (ulong time, double energy) firstPair =
                    (firstTuples.ElementAt(i).DataTuple.Item1,
                    firstTuples.ElementAt(i).DataTuple.Item2);
                (ulong time, double energy) secondPair =
                    (secondTuples.ElementAt(i).DataTuple.Item1,
                    secondTuples.ElementAt(i).DataTuple.Item2);

                tmpSum += (secondPair.energy - firstPair.energy) / (secondPair.time - firstPair.time) * 3_600_000;
            }

            return tmpSum / length;
        }
    }
}