using GiaPha_Application.Common;
using GiaPha_Application.DTOs;
using MediatR;

namespace GiaPha_Application.Features.HoName.Command.UpdateHo;
public record UpdateHoCommand : IRequest<Result<HoResponse>>
{
    public Guid Id { get; init; }
    public Guid? ThuyToId { get; init; }
    public string TenHo { get; init; } = null!;
    public string? MoTa { get; init; }
    public string? queQuan { get; init; }
    public string? hinhAnh { get; init; }
}