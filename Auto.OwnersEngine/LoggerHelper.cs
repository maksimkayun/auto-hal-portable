using Auto.Data;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace Auto.OwnersEngine;

public class LoggerHelper
{
    public static ILogger<AutoCsvFileDatabase> GetConsoleLogger()
    {
        var configureNamedOptions = new ConfigureNamedOptions<ConsoleLoggerOptions>("", null);
        var optionsFactory = new OptionsFactory<ConsoleLoggerOptions>(new []{ configureNamedOptions }, Enumerable.Empty<IPostConfigureOptions<ConsoleLoggerOptions>>());
        var optionsMonitor = new OptionsMonitor<ConsoleLoggerOptions>(optionsFactory, Enumerable.Empty<IOptionsChangeTokenSource<ConsoleLoggerOptions>>(), new OptionsCache<ConsoleLoggerOptions>());
        var loggerFactory = new LoggerFactory(new[] { new ConsoleLoggerProvider(optionsMonitor) }, new LoggerFilterOptions { MinLevel = LogLevel.Information });
        return loggerFactory.CreateLogger<AutoCsvFileDatabase>();
    }
}