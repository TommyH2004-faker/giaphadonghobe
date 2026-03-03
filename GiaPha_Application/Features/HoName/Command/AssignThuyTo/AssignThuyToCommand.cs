using GiaPha_Application.Common;
using GiaPha_Application.DTOs;
using MediatR;
namespace GiaPha_Application.Features.HoName.Command.AssignThuyTo;
public record AssignThuyToCommand : IRequest<Result<HoResponse>>
{
    public Guid HoId { get; init; }
    public Guid ThuyToId { get; init; }
}