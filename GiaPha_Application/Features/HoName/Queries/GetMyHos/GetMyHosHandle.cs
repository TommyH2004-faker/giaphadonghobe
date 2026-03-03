using GiaPha_Application.Common;
using GiaPha_Application.DTOs;
using GiaPha_Application.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GiaPha_Application.Features.HoName.Queries.GetMyHos;

public class GetMyHosHandle : IRequestHandler<GetMyHosQuery, Result<List<HoResponse>>>
{
    private readonly IHoRepository _hoRepository;
    private readonly IGiaPhaRepository _giaPhaRepository;
    private readonly ILogger<GetMyHosHandle> _logger;

    public GetMyHosHandle(
        IHoRepository hoRepository,
        IGiaPhaRepository giaPhaRepository,
        ILogger<GetMyHosHandle> logger)
    {
        _hoRepository = hoRepository;
        _giaPhaRepository = giaPhaRepository;
        _logger = logger;
    }

    public async Task<Result<List<HoResponse>>> Handle(GetMyHosQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting Ho by HoId: {HoId}", request.HoId);

        var hoResult = await _hoRepository.GetHoByIdAsync(request.HoId);
        
        if (!hoResult.IsSuccess || hoResult.Data == null)
        {
            _logger.LogWarning("Ho not found: {HoId}", request.HoId);
            return Result<List<HoResponse>>.Success(new List<HoResponse>());
        }

        var ho = hoResult.Data;

        // Build tree để đếm chính xác số thành viên (bao gồm cả vợ/chồng từ họ khác)
        int tongSoThanhVien = 0;
        try
        {
            var treeResult = await _giaPhaRepository.BuildGiaPhaTreeAsync(
                request.HoId,
                maxLevel: 100,
                includeNuGioi: true,
                includeDeleted: false // Không đếm người đã xóa
            );
            
            if (treeResult.IsSuccess && treeResult.Data != null)
            {
                tongSoThanhVien = treeResult.Data.TongSoThanhVien;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not build tree for Ho {HoId}, using fallback count", request.HoId);
            tongSoThanhVien = ho.ThanhViens?.Count ?? 0;
        }

        var hoResponse = new HoResponse
        {
            Id = ho.Id,
            TenHo = ho.TenHo,
            MoTa = ho.MoTa,
            HinhAnh = ho.HinhAnh,
            QueQuan = ho.QueQuan,
            ThuyToId = ho.ThuyToId,
            SoThanhVien = tongSoThanhVien
        };

        _logger.LogInformation("Found Ho: {HoId} - {TenHo} with {Count} members", ho.Id, ho.TenHo, tongSoThanhVien);
        return Result<List<HoResponse>>.Success(new List<HoResponse> { hoResponse });
    }
}
