using GiaPha_Application.Common;
using GiaPha_Application.DTOs;
using MediatR;

namespace GiaPha_Application.Features.HoName.Command.CreateHo;

public record CreateHoCommand : IRequest<Result<HoResponse>>
{
    // Thông tin User tạo họ
    public Guid UserId { get; init; }
    
    // Thông tin Họ
    public string TenHo { get; init; } = null!;
    public string? MoTa { get; init; }
    public string? QueQuan { get; init; }
    
    // Thông tin Thành viên Thủy Tổ
    public string HoTenThuyTo { get; init; } = null!;
    public bool GioiTinhThuyTo { get; init; }
    public DateTime NgaySinhThuyTo { get; init; }
    public string? NoiSinhThuyTo { get; init; }
    public string? TieuSuThuyTo { get; init; }
}