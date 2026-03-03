using GiaPha_Application.Common;
using MediatR;

namespace GiaPha_Application.Features.Auth.Command.Activate;
public record ActivateUserCommand : IRequest<Result<bool>>
{
    public Guid UserId { get; init; }
    public string ActivationCode { get; init; } = null!;
}