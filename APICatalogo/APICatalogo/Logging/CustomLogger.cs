namespace APICatalogo.Logging;

public class CustomLogger(string loggerName, CustomLoggerProviderConfiguration loggerConfig) : ILogger
{
    readonly string _loggerName = loggerName;
    readonly CustomLoggerProviderConfiguration _loggerConfig = loggerConfig;

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        => null;

    public bool IsEnabled(LogLevel logLevel)
        => logLevel == _loggerConfig.LogLevel; 

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        string mensagem = $"{logLevel}: {eventId.Id} = {formatter(state, exception)}";

        EscreverTextoNoArquivo(mensagem);
    }

    private void EscreverTextoNoArquivo(string mensagem)
    {
        string caminhoArquivoLog = AppDomain.CurrentDomain.BaseDirectory + "Catalogo_api.txt";

        if (!File.Exists(caminhoArquivoLog))
            File.Create(caminhoArquivoLog);

        using StreamWriter writer = new(caminhoArquivoLog, true);
        try
        {
            writer.WriteLine(mensagem);
            writer.Close();
        }
        catch (Exception)
        {
            throw;
        }

    }
}
  