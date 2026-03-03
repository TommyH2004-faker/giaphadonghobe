using GiaPha_Application.Common;
using GiaPha_Application.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GiaPha_Application.Features.Notification.Commands.MarkAsRead;

public class MarkAsReadHandler : IRequestHandler<MarkAsReadCommand, Result<bool>>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<MarkAsReadHandler> _logger;

    public MarkAsReadHandler(
        INotificationRepository notificationRepository,
        IUnitOfWork unitOfWork,
        ILogger<MarkAsReadHandler> logger)
    {
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(
        MarkAsReadCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("✏️ [Command] User {UserId} marking notification {NotificationId} as read", 
                request.UserId, request.NotificationId);

            // Đánh dấu đã đọc
            await _notificationRepository.MarkAsReadAsync(request.NotificationId);

            // Persist to database
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("✅ [Command] Successfully marked notification {NotificationId} as read", 
                request.NotificationId);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error marking notification {NotificationId} as read", 
                request.NotificationId);
            
            return Result<bool>.Failure(
                ErrorType.InternalServerError,
                "Lỗi khi cập nhật trạng thái thông báo");
        }
    }
}
