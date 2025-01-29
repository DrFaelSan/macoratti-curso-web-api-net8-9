using System.Collections.Concurrent;

namespace APICatalogo.Logging;

public class CustomLoggerProvider : ILoggerProvider
{
    readonly CustomLoggerProviderConfiguration _loggerConfig;
    readonly ConcurrentDictionary<string, CustomLogger> _loggers = new();

    public CustomLoggerProvider(CustomLoggerProviderConfiguration config)
    {
        _loggerConfig = config;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return _loggers.GetOrAdd(categoryName, name => new CustomLogger(name, _loggerConfig));
    }

    public void Dispose()
    {
        _loggers.Clear();
    }
}
