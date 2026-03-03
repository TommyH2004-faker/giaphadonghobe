using GiaPha_Application.Common;
using GiaPha_Application.DTOs;
using MediatR;

namespace GiaPha_Application.Features.HoName.Queries.GetById;
public record GetHoByIdQuery() : IRequest<Result<HoResponse>>
{
    public Guid Id { get; init; }
}