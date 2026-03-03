using GiaPha_Application.Common;
using GiaPha_Application.DTOs;
using MediatR;

namespace GiaPha_Application.Features.HoName.Queries.GetHoByUserEmail;

public record GetHoByUserEmailQuery(string Email) : IRequest<Result<HoResponse>>;
