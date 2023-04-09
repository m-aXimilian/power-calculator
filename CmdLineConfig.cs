internal static class CmdLineConfig
{
    static internal readonly string GeneralDocumentation =
    @"A consumer of a vzlogger REST-API packet, that will perform some statistics 
based on channels that are identified in the vzloggerOptions config-file.
It will deduce a mean power from the meter readings of the specified channels 
and compare a current power against it. Upon predefined threshold values, it will
signal warnings to any attached listeners.";

    static internal readonly string ConfigOptionsIdentifier = "--config";

    static internal readonly string ConfigOptionsDescription =
    @"Absolute path to the domain logic configuration json-file. It should provide parameters for
    - MovingAverageNumber
    - WaitbetweenApiCall
    - VzLoggerChannelFilter
    - WarnThreshold
    - ErrorThreshold";

    static internal readonly string VzloggerOptionsIdentifier = "--vzloggerconf";

    static internal readonly string VzloggerOptionsDescription =
    @"Absolute path to the Vzlogger configuraion json-file. It should provide prameters for
    - ApiUrl
    - A dictionary with some channel IDs (with none specified, the whole application makes no sense)";

    static internal readonly string LogOptionsIdentifier = "--log";

    static internal readonly string LogOptionsDescription = "The absolute path to the log-file that should be used.";

    static internal readonly string LoggerLevelDebugFlag = "--debug";

    static internal readonly string LoggerLevelDebugDescription = "When given, the log level is set to debug. This will add debug-level info to the log file.";
}