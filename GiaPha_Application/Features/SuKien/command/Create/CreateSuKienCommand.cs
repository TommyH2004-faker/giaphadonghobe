using GiaPha_Application.Common;
using GiaPha_Application.DTOs;
using MediatR;

namespace GiaPha_Application.Features.SuKien.Command.Create;
public record CreateSuKienCommand(
    Guid ThanhVienId,
    string LoaiSuKien,
    DateTime NgayXayRa,
    string? DiaDiem,
    string? MoTa) : IRequest<Result<SuKienResponse>>;