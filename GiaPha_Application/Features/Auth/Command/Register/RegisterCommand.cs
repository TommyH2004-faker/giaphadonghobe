using GiaPha_Application.Common;
using GiaPha_Application.DTOs;
using MediatR;

namespace GiaPha_Application.Features.Auth.Command.Register;

public record RegisterCommand: IRequest<Result<UserResponse>>
{
    public string TenDangNhap { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string MatKhau { get; init; } = null!;
    public string? SoDienThoai { get; init; }
    public bool GioiTinh { get; init; }
}