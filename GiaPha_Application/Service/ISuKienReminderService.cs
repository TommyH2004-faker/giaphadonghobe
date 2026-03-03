namespace GiaPha_Application.Service;

/// <summary>
/// Interface cho dịch vụ gửi email nhắc nhở sự kiện.
/// Thuộc Application Layer (Clean Architecture).
/// </summary>
public interface ISuKienReminderService
{
    /// <summary>
    /// Kiểm tra và gửi email nhắc nhở cho các sự kiện diễn ra sau đúng 3 ngày.
    /// </summary>
    Task SendUpcomingEventRemindersAsync(CancellationToken cancellationToken = default);
}
