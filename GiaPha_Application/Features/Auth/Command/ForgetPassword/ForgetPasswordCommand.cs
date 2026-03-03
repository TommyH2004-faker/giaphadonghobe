using GiaPha_Application.Common;
using MediatR;

namespace GiaPha_Application.Features.Auth.Command.ForgetPassword;
public record ForgetPasswordCommand : IRequest<Result<bool>>
{
    public string Email { get; init; } = null!;
    public Guid Id { get; init;  }
}