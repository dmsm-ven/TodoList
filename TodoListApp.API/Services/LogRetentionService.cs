using TodoListApp.Core.Repositories.Interfaces;

namespace TodoListApp.API.Services;

/// <summary>
/// Раз в сутки удаляет из журнала запросов (log_entry) записи старше LogRetentionDays дней (по умолчанию 30).
/// </summary>
public class LogRetentionService(
    IDapperDatabaseAccess database,
    IConfiguration configuration,
    ILogger<LogRetentionService> logger) : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromHours(24);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var retentionDays = Math.Max(1, configuration.GetValue("LogRetentionDays", 30));

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var cutoff = DateTime.SpecifyKind(DateTime.UtcNow.AddDays(-retentionDays), DateTimeKind.Unspecified);
                await database.ExecuteAsync("DELETE FROM log_entry WHERE date_time < @cutoff", new { cutoff });
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Не удалось очистить старые записи журнала");
            }

            try
            {
                await Task.Delay(Interval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }
}
