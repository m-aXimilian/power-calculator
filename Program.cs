namespace Power.Calculator
{
    using Rest.Api;
    using Domain.Meter.Configuration;
    using Domain.PowerMeter;
    using Serilog;
    using Newtonsoft.Json;
    using System.CommandLine;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using Power.Calculator.UdpSocket;
    using System.Net;

    public class Program
    {
        private static async Task Main(string[] args)
        {
            var configuration = new Option<string>(
                name: CmdLineConfig.ConfigOptionsIdentifier,
                description: CmdLineConfig.ConfigOptionsDescription);
            var vzloggerOptions = new Option<string>(name: CmdLineConfig.VzloggerOptionsIdentifier,
                description: CmdLineConfig.VzloggerOptionsDescription);
            var logger = new Option<string>(name: CmdLineConfig.LogOptionsIdentifier,
                description: CmdLineConfig.LogOptionsDescription);
            var logLevel = new Option<bool>(name: CmdLineConfig.LoggerLevelDebugFlag,
                description: CmdLineConfig.LoggerLevelDebugDescription);

            var rootCommand = new RootCommand(CmdLineConfig.GeneralDocumentation);
            rootCommand.AddOption(configuration);
            rootCommand.AddOption(vzloggerOptions);
            rootCommand.AddOption(logger);
            rootCommand.AddOption(logLevel);
            rootCommand.SetHandler(async (domainOptions, vzloggerOptions, log, lvl) =>
            {
                await Run(domainOptions: domainOptions, vzloggerOptions: vzloggerOptions, logger: log, level: lvl);
            }, configuration, vzloggerOptions, logger, logLevel);

            await rootCommand.InvokeAsync(args);
        }

        private static async Task Run(string domainOptions, string vzloggerOptions, string logger, bool level)
        {
            const int loggerSizeLimit = 100_000;
            if (level)
            {
                Log.Logger = new LoggerConfiguration()
                                    .MinimumLevel.Debug()
                                    .WriteTo.File(logger, rollOnFileSizeLimit: true, fileSizeLimitBytes: loggerSizeLimit)
                                    .CreateLogger();
            }
            else
            {
                Log.Logger = new LoggerConfiguration()
                                    .WriteTo.File(logger, rollOnFileSizeLimit: true, fileSizeLimitBytes: loggerSizeLimit)
                                    .CreateLogger();
            }

            Log.Information("Starting Server...");

            var domainConfiguration = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(domainOptions));

            MeterConfigurationWrapper meter = new(vzloggerOptions);

            var channelFilter = domainConfiguration["VzLoggerChannelFilter"];
            var channelsProduction = meter.MeterConfiguration.Channels.Where(k => k.Key.StartsWith(channelFilter))
            .ToDictionary(i => i.Key, i => i.Value);
            var serverAddress = domainConfiguration["ServerAddress"];
            var serverPort = domainConfiguration["ServerPort"];

            using (var api = new RestAPIMock(meter.MeterConfiguration.ApiUrl, meter.MeterConfiguration.Channels))
            {
                MeterEntityController productionController = new(
                    api,
                    (int)float.Parse(domainConfiguration["MovingAverageNumber"]),
                    EnergyDirection.PRODUCTION, channelsProduction);
                // this is needed because it does the calculation based on events of the productionController
                MeanPowerCalculator calcProduction = new(productionController);
                IPEndPoint endPoint = new(
                    IPAddress.Parse(serverAddress),
                    int.Parse(serverPort));

                var udpServer = new PowerCalculatorSocket(endPoint, productionController, calcProduction);
                udpServer.StartServer();
                await productionController.CallLoop((int)float.Parse(domainConfiguration["WaitbetweenApiCall"]) * 1_000);
            }
        }
    }
}

