using GiaPha_Application.Common;
using MediatR;

namespace GiaPha_Application.Features.Notification.Commands.CreateNotification;
public record CreateNotificationCommand(
    Guid UserId,
    string NoiDung,
    Guid HoId
) : IRequest<Result<Guid>>;