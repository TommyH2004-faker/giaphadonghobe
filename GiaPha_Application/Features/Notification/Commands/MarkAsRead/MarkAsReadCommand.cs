using GiaPha_Application.Common;
using MediatR;

namespace GiaPha_Application.Features.Notification.Commands.MarkAsRead;

/// <summary>
/// Command đánh dấu notification đã đọc
/// </summary>
public record MarkAsReadCommand(
    Guid NotificationId,
    Guid UserId // Để log tracking
) : IRequest<Result<bool>>;
