using GiaPha_Application.Common;
using GiaPha_Application.DTOs;
using MediatR;

namespace GiaPha_Application.Features.HoName.Queries.GetAllHo;
public record GetAllHoQuery() : IRequest<Result<List<HoResponse>>>
{
}