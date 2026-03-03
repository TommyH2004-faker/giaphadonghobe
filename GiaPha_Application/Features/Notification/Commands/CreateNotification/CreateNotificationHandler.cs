using GiaPha_Application.Common;
using GiaPha_Application.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GiaPha_Application.Features.Notification.Commands.CreateNotification;

public class CreateNotificationHandler : IRequestHandler<CreateNotificationCommand, Result<Guid>>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateNotificationHandler> _logger;

    public CreateNotificationHandler(
        INotificationRepository notificationRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateNotificationHandler> logger)
    {
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var notification = GiaPha_Domain.Entities.Notification.Create(
                noiDung: request.NoiDung,
                isGlobal: false,
                nguoiNhanId: null,
                hoId: request.HoId
            );

            await _notificationRepository.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("✅ [Notification] Trưởng họ {UserId} tạo thông báo {NotificationId} cho họ {HoId}",
                request.UserId, notification.Id, request.HoId);

            return Result<Guid>.Success(notification.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [Notification] Error creating notification");
            return Result<Guid>.Failure(ErrorType.InternalServerError, "Lỗi khi tạo thông báo");
        }
    }
}
   