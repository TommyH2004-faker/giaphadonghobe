using GiaPha_Application.Common;
using MediatR;

namespace GiaPha_Application.Features.YeuCau.Queries.GetPendingRequests;

public record GetPendingRequestsQuery(Guid HoId) : IRequest<Result<IReadOnlyList<YeuCauDto>>>;

public record YeuCauDto(
    Guid Id,
    Guid UserId,
    string Email,
    string TenDangNhap,
    string? Avatar,
    string LyDoXinVao,
    DateTime NgayTao
);
