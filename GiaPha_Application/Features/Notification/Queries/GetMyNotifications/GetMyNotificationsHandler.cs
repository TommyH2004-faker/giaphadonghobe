using GiaPha_Application.Common;
using GiaPha_Application.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GiaPha_Application.Features.Notification.Queries.GetMyNotifications;

public class GetMyNotificationsHandler 
    : IRequestHandler<GetMyNotificationsQuery, Result<IReadOnlyList<GiaPha_Domain.Entities.Notification>>>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly ILogger<GetMyNotificationsHandler> _logger;

    public GetMyNotificationsHandler(
        INotificationRepository notificationRepository,
        ILogger<GetMyNotificationsHandler> logger)
    {
        _notificationRepository = notificationRepository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<GiaPha_Domain.Entities.Notification>>> Handle(
        GetMyNotificationsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("🔍 [Query] Getting notifications for userId={UserId}, hoId={HoId}", 
                request.UserId, request.HoId);

            // Lấy notifications theo filter (isGlobal hoặc hoId)
            var notifications = await _notificationRepository.GetAllForUserAsync(
                request.UserId, 
                request.HoId);

            _logger.LogInformation("✅ Found {Count} notifications", notifications.Count);

            return Result<IReadOnlyList<GiaPha_Domain.Entities.Notification>>.Success(notifications);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error getting notifications for user {UserId}", request.UserId);
            return Result<IReadOnlyList<GiaPha_Domain.Entities.Notification>>.Failure(
                ErrorType.InternalServerError,
                "Lỗi khi lấy thông báo");
        }
    }
}
