using GiaPha_Application.Common;
using MediatR;

namespace GiaPha_Application.Features.ThanhVien.Command.UpdateAvatar;

public record UpdateAvatarCommand : IRequest<Result<string>>
{
    public Guid ThanhVienId { get; init; }
    public string AvatarUrl { get; init; } = null!;
}
