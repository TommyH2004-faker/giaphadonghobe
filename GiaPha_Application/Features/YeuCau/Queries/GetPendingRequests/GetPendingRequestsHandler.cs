using GiaPha_Application.Common;
using GiaPha_Application.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GiaPha_Application.Features.YeuCau.Queries.GetPendingRequests;

public class GetPendingRequestsHandler : IRequestHandler<GetPendingRequestsQuery, Result<IReadOnlyList<YeuCauDto>>>
{
    private readonly IYeuCauThamGiaHoRepository _repo;
    private readonly ILogger<GetPendingRequestsHandler> _logger;

    public GetPendingRequestsHandler(IYeuCauThamGiaHoRepository repo, ILogger<GetPendingRequestsHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<YeuCauDto>>> Handle(GetPendingRequestsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var list = await _repo.GetPendingByHoIdAsync(request.HoId);

            var dtos = list.Select(y => new YeuCauDto(
                y.Id,
                y.UserId,
                y.User?.Email ?? "",
                y.User?.TenDangNhap ?? "",
                y.User?.Avatar,
                y.LyDoXinVao,
                y.NgayTao
            )).ToList();

            return Result<IReadOnlyList<YeuCauDto>>.Success(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Lỗi khi lấy danh sách yêu cầu tham gia họ {HoId}", request.HoId);
            return Result<IReadOnlyList<YeuCauDto>>.Failure(ErrorType.InternalServerError, "Lỗi khi lấy danh sách yêu cầu");
        }
    }
}
