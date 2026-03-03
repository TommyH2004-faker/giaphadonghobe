using GiaPha_Application.Common;
using GiaPha_Application.DTOs;
using MediatR;

namespace GiaPha_Application.Features.SuKien.Queries.GetAllSuKien;
public record GetAllSuKienQuery() 
    : IRequest<Result<List<SuKienResponse>>>;