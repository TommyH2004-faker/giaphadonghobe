using GiaPha_Application.Common;
using GiaPha_Domain.Entities;
using MediatR;

namespace GiaPha_Application.Features.Notification.Queries.GetMyNotifications;

/// <summary>
/// Query lấy tất cả notifications của user hiện tại
/// </summary>
public record GetMyNotificationsQuery(
    Guid UserId,
    Guid? HoId
) : IRequest<Result<IReadOnlyList<GiaPha_Domain.Entities.Notification>>>;
