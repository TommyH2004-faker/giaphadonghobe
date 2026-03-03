using GiaPha_Application.Common;
using MediatR;

namespace GiaPha_Application.Features.Auth.Command.Changepassword.ChangePasswordCommand;
public record ChangePasswordCommand : IRequest<Result<bool>>
{
    public Guid UserId { get; init; }
    public string CurrentPassword { get; init; } = null!;
    public string NewPassword { get; init; } = null!;
}