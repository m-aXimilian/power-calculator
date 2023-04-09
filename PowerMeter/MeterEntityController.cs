using Rest.Api;
using Serilog;

namespace Domain.PowerMeter
{
    public class MeterEntityController : IMeterEntity
    {
        private readonly IRestGet restApi;
        private IMeterCollection lastFinishedCollection;
        public event EventHandler NewMeterCollectionAvailable;

        private int maxReadingSize;

        public MeterEntityController(IRestGet api,
                                     int maxReadingSize,
                                     EnergyDirection channelType,
                                     IDictionary<string, string> channels)
        {
            this.restApi = api ?? throw new ArgumentNullException(nameof(api));
            this.maxReadingSize = maxReadingSize;
            this.ChannelType = channelType;
            this.ChannelCollecton = channels;
            this.MeterReadings = new();
            this.NewMeterCollectionAvailable += this.HandleNewMeterCollection_DoWork;
        }

        public IDictionary<string, string> ChannelCollecton { get; }

        public EnergyDirection ChannelType { get; }

        public List<IMeterCollection> MeterReadings { get; set; }

        private IMeterCollection LastFinishedCollection
        {
            get => this.lastFinishedCollection;
            set
            {
                if (value != null)
                {
                    this.lastFinishedCollection = value;
                    this.OnLastCollectionChanged();
                }
            }
        }

        private long CreationTime = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();

        public async Task CallLoop(int wait)
        {
            while (true)
            {
                await this.GenerateNewCollection();
                await Task.Run(() => Thread.Sleep(wait));
            }
        }

        public async Task GenerateNewCollection()
        {
            var apiCall = await this.restApi.UpdateRestReults(this.ChannelCollecton);
            this.LastFinishedCollection = new PowerMeterCollection(apiCall);
        }

        public void OnLastCollectionChanged()
        {
            var del = NewMeterCollectionAvailable as EventHandler;
            if (del != null)
            {
                del(this, EventArgs.Empty);
            }
        }

        private void HandleNewMeterCollection_DoWork(object sender, EventArgs e)
        {
            Log.Debug($"{nameof(MeterEntityController)}: {sender}[{this.ChannelType}]: new reading set after {this.LastFinishedCollection.Timestamp - this.CreationTime}s with ID {this.LastFinishedCollection.Id}.");
            if (this.MeterReadings.Count < this.maxReadingSize)
            {
                this.MeterReadings.Add(this.lastFinishedCollection);
                return;
            }

            this.MeterReadings.RemoveAt(0);
            this.MeterReadings.Add(this.lastFinishedCollection);
        }
    }
}