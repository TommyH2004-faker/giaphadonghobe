using GiaPha_Application.Common;
using GiaPha_Application.DTOs;
using MediatR;

namespace GiaPha_Application.Features.ThanhVien.Command.Create;
public record CreateThanhVienCommand() : IRequest<Result<ThanhVienResponse>>
{
    public string HoTen { get; init; } = null!;
    public bool GioiTinh { get; init; }
    public DateTime NgaySinh { get; init; }
    public string NoiSinh { get; init; } = null!;
    public bool TrangThai { get; init; }
    public Guid? HoId { get; init; } // Nullable: vợ/chồng có thể từ họ khác
    public string? TieuSu { get; init; }
    public Guid? ParentId { get; init; } // ID của cha/mẹ (để tự động tạo quan hệ)
}