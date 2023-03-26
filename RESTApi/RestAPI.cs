using System.Text.Json;
using DataTypes.Data;
using DataTypes.Data.Vzlogger;
using DataTypes.Data.Vzlogger.Packet;


namespace Rest.Api
{
    public class RestAPI : IDisposable, IRestGet
    {
        private bool disposedValue;
        private HttpClient httpClient;

        public RestAPI(string url, IDictionary<string, string> channels)
        {
            this.URL = url;
            this.Channels = channels;
            this.LastRestResults = new();
            this.httpClient = new HttpClient();
        }

        public List<IAbstractData> LastRestResults { get; private set; }

        public string URL { get; }

        /// <summary>
        /// Used as a fallback for <see cref="UpdateRestReults"/> without parameters.
        /// </summary>
        public IDictionary<string, string> Channels { get; }

        /// <summary>
        /// Updates the list of last results. Use this to get values for all available 
        /// channels. 
        /// </summary>
        /// <returns>The list of new Logger data.</returns>
        public async Task<List<IAbstractData>> UpdateRestReults()
        {
            List<IAbstractData> resultList = new();
            foreach (var pair in this.Channels)
            {
                resultList.Add(new VzloggerData(await this.GetRestResponse(pair.Value)));
            }

            this.LastRestResults = resultList;
            return resultList;
        }

        /// <summary>
        /// Creates a list of <see cref="VzloggerData"> from async API calls.
        /// </summary>
        /// <param name="channels">A dictionary of channels to query.</param>
        /// <returns>List of <see cref="VzloggerData"> data.</returns>
        /// <exception cref="ArgumentException">Throws if the input dictionary is empty.</exception>
        public async Task<List<IAbstractData>> UpdateRestReults(IDictionary<string, string> channels)
        {
            if (channels.Count < 1)
            {
                throw new ArgumentException($"{nameof(channels)} should contain at least one element.");
            }

            List<IAbstractData> resultList = new();
            foreach (var pair in channels)
            {
                resultList.Add(new VzloggerData(await this.GetRestResponse(pair.Value)));
            }

            return resultList;
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.httpClient.Dispose();
                }

                disposedValue = true;
            }
        }

        private async Task<VzloggerPacket> GetRestResponse(string channel)
        {
            var restResponse = await this.httpClient.GetStringAsync($"{this.URL}/{channel}");
            return JsonSerializer.Deserialize<VzloggerPacket>(restResponse);
        }
    }
}
