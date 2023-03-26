using System.Configuration;
using Newtonsoft.Json;
using Serilog;

namespace Domain.Meter.Configuration
{
    public class MeterConfigurationWrapper
    {
        private MeterConfiguration meterConfiguration;
        public MeterConfigurationWrapper(string configurationFilePath)
        {
            try
            {
                var tmp = File.ReadAllText(configurationFilePath);
                this.meterConfiguration = JsonConvert.DeserializeObject<MeterConfiguration>(tmp);
            }
            catch (Exception e)
            {
                Log.Error($"{nameof(MeterConfigurationWrapper)}: Configuration did not work {e}");
            }
        }

        public MeterConfiguration MeterConfiguration => this.meterConfiguration;
    }
}