namespace Power.Calculator
{
    using Rest.Api;
    using Domain.Meter.Configuration;
    using Domain.PowerMeter;
    using Serilog;
    using Newtonsoft.Json;
    using System.CommandLine;
    using System.Threading.Tasks;
    using System;
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

            var rootCommand = new RootCommand(CmdLineConfig.GeneralDocumentation);
            rootCommand.AddOption(configuration);
            rootCommand.AddOption(vzloggerOptions);
            rootCommand.AddOption(logger);
            rootCommand.SetHandler(async (domainOptions, vzloggerOptions, log) =>
            {
                await Run(domainOptions: domainOptions, vzloggerOptions: vzloggerOptions, logger: log);
            }, configuration, vzloggerOptions, logger);

            await rootCommand.InvokeAsync(args);
        }

        private static async Task Run(string domainOptions, string vzloggerOptions, string logger)
        {
            const int loggerSizeLimit = 100_000;
            Log.Logger = new LoggerConfiguration()
                                    .WriteTo.File(logger, rollOnFileSizeLimit: true, fileSizeLimitBytes: loggerSizeLimit)
                                    .CreateLogger();


            Console.WriteLine($"options: {domainOptions}, logger: {logger}");
            var domainConfiguration = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(domainOptions));

            MeterConfigurationWrapper meter = new(vzloggerOptions);

            var channelFilter = domainConfiguration["VzLoggerChannelFilter"];
            var channelsProduction = meter.MeterConfiguration.Channels.Where(k => k.Key.StartsWith(channelFilter))
            .ToDictionary(i => i.Key, i => i.Value);

            using (var api = new RestAPIMock(meter.MeterConfiguration.ApiUrl, meter.MeterConfiguration.Channels))
            {
                Log.Information("Starting...");
                MeterEntityController productionController = new(
                    api,
                    (int)float.Parse(domainConfiguration["MovingAverageNumber"]),
                    EnergyDirection.PRODUCTION, channelsProduction);
                // this is needed because it does the calculation based on events of the productionController
                MeanPowerCalculator calcProduction = new(productionController);
                IPEndPoint endPoint = new(
                    IPAddress.Parse("127.0.0.1"),
                    8_403);

                var udpServer = new PowerCalculatorSocket(endPoint, productionController, calcProduction);
                udpServer.StartServer();
                await productionController.CallLoop((int)float.Parse(domainConfiguration["WaitbetweenApiCall"]) * 1_000);

            }
        }
    }
}

