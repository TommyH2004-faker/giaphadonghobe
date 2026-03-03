using GiaPha_Application.Common;
using GiaPha_Application.DTOs;
using MediatR;

namespace GiaPha_Application.Features.ThanhVien.Queries.GetByHoid;
public record GetThanhVienByHoIdQuery(Guid HoId) : IRequest<List<ThanhVienResponse>>;