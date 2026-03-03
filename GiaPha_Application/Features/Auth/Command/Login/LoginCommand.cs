using GiaPha_Application.Common;
using GiaPha_Application.DTOs;
using MediatR;

namespace GiaPha_Application.Features.Auth.Command.Login;
public record LoginCommand : IRequest<Result<LoginRespone>>
{
    public string TenDangNhap { get; init; } = null!;
    public string MatKhau { get; init; } = null!;
    public string Email { get; init; } = null!;
    public bool Enabled { get; init; }
}