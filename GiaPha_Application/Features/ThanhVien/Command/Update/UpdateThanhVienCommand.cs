using GiaPha_Application.Common;
using GiaPha_Application.DTOs;
using MediatR;

namespace GiaPha_Application.Features.ThanhVien.Command.Update;

public record UpdateThanhVienCommand : IRequest<Result<ThanhVienResponse>>
{
    public Guid Id { get; init; }
    public string HoTen { get; init; } = null!;
    public bool GioiTinh { get; init; }
    public DateTime NgaySinh { get; init; }
    public string NoiSinh { get; init; } = null!;
    public DateTime? NgayMat { get; init; }
    public string? NoiMat { get; init; }
    public string? TieuSu { get; init; }
    public bool TrangThai { get; init; }
}
