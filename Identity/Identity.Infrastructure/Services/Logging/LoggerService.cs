using Serilog;
using ILogger = Serilog.ILogger;

namespace Identity.Infrastructure.Services.Logging;

public class LoggerService
{
    private readonly ILogger _logger = new LoggerConfiguration()
        .WriteTo.Console()
        .WriteTo.File("Logs/identity_server_.txt", rollingInterval: RollingInterval.Day)
        .CreateLogger();

    public void LogInformation(string message) => _logger.Information(message);
    public void LogWarning(string message) => _logger.Warning(message);
    public void LogError(string message, Exception ex) => _logger.Error(ex, message);
    public void LogDebug(string message) => _logger.Debug(message);
}
