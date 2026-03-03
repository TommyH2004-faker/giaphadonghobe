using GiaPha_Application.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GiaPha_WebAPI.BackgroundServices;

/// <summary>
/// Hosted BackgroundService chạy mỗi ngày.
/// Thuộc WebAPI Layer (Clean Architecture) — chỉ đóng vai trò orchestrator,
/// ủy quyền toàn bộ business logic cho ISuKienReminderService ở Application Layer.
/// </summary>
public class SuKienEmailReminderBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SuKienEmailReminderBackgroundService> _logger;

    private static readonly TimeSpan Interval = TimeSpan.FromHours(24);

    public SuKienEmailReminderBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<SuKienEmailReminderBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("📧 SuKienEmailReminderBackgroundService started. Interval: {Hours}h.", Interval.TotalHours);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var reminderService = scope.ServiceProvider.GetRequiredService<ISuKienReminderService>();
                await reminderService.SendUpcomingEventRemindersAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error in SuKienEmailReminderBackgroundService");
            }

            await Task.Delay(Interval, stoppingToken);
        }
    }
}
