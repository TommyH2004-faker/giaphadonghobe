using GiaPha_Application.Common;
using GiaPha_Application.DTOs;
using MediatR;

namespace GiaPha_Application.Features.HoName.Queries.GetMyHos;

public record GetMyHosQuery(Guid HoId) : IRequest<Result<List<HoResponse>>>;
