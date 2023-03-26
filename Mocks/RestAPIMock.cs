using DataTypes.Data;
using DataTypes.Data.Vzlogger;
using DataTypes.Data.Vzlogger.Packet;


namespace Rest.Api
{
    public class RestAPIMock : IDisposable, IRestGet
    {
        private double energy = 5_000.0f;
        public RestAPIMock(string url, IDictionary<string, string> channels)
        {
            this.URL = url;
            this.Channels = channels;
            this.LastRestResults = new();
        }

        public List<IAbstractData> LastRestResults { get; set; }

        public string URL { get; }

        public IDictionary<string, string> Channels { get; }

        public void Dispose()
        {
        }

        public async Task<List<IAbstractData>> UpdateRestReults()
        {
            List<IAbstractData> resultList = new();
            foreach (var pair in this.Channels)
            {
                resultList.Add(new VzloggerData(await this.MockVzloggerData(pair.Value)));
            }

            this.LastRestResults = resultList;
            return resultList;
        }

        public async Task<List<IAbstractData>> UpdateRestReults(IDictionary<string, string> channels)
        {
            List<IAbstractData> resultList = new();
            foreach (var pair in channels)
            {
                resultList.Add(new VzloggerData(await this.MockVzloggerData(pair.Value)));
            }

            return resultList;
        }

        private async Task<VzloggerPacket> MockVzloggerData(string channel)
        {
            var rnd = new Random();
            long timeNow = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            this.energy += ((float)new Decimal(rnd.NextDouble() * 5));
            VzloggerPacket packet = new VzloggerPacket
            {
                version = "0.0",
                generator = "mock",
                data = new List<Data>
                {
                    new Data
                    {
                        uuid = channel,
                        last = timeNow,
                        interval = 0,
                        protocol = "vzlogger",
                        tuples = new List<List<double>>
                        {
                            new List<double>{timeNow, this.energy}
                        }
                    }
                }
            };

            await Task.Run(() => Thread.Sleep(100));
            return packet;
        }
    }
}