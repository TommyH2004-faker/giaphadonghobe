using GiaPha_Application.Common;
using GiaPha_Application.DTOs;
using MediatR;

namespace GiaPha_Application.Features.ThanhVien.Queries.GetById;

public record GetThanhVienByIdQuery(Guid Id) : IRequest<Result<ThanhVienResponse>>;
