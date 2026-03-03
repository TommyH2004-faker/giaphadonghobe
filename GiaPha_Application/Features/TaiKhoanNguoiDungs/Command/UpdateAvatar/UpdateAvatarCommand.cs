using GiaPha_Application.Common;
using MediatR;

namespace GiaPha_Application.Features.TaiKhoanNguoiDungs.Command.UpdateAvatar;
public record UpdateAvatarCommand(Guid Id, string Avatar) : IRequest<Result<bool>>;