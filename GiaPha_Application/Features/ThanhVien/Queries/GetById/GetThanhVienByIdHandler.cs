using GiaPha_Application.Common;
using GiaPha_Application.DTOs;
using GiaPha_Application.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GiaPha_Application.Features.ThanhVien.Queries.GetById;

public class GetThanhVienByIdHandler : IRequestHandler<GetThanhVienByIdQuery, Result<ThanhVienResponse>>
{
    private readonly IThanhVienRepository _thanhVienRepository;
    private readonly ILogger<GetThanhVienByIdHandler> _logger;

    public GetThanhVienByIdHandler(
        IThanhVienRepository thanhVienRepository,
        ILogger<GetThanhVienByIdHandler> logger)
    {
        _thanhVienRepository = thanhVienRepository;
        _logger = logger;
    }

    public async Task<Result<ThanhVienResponse>> Handle(GetThanhVienByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("🔍 [GetThanhVienById] Getting member: {Id}", request.Id);

        var memberResult = await _thanhVienRepository.GetThanhVienByIdAsync(request.Id);
        
        if (!memberResult.IsSuccess || memberResult.Data == null)
        {
            _logger.LogWarning("⚠️ [GetThanhVienById] Member not found: {Id}", request.Id);
            return Result<ThanhVienResponse>.Failure(ErrorType.NotFound, "Không tìm thấy thành viên");
        }

        var member = memberResult.Data;

        var response = new ThanhVienResponse
        {
            Id = member.Id,
            Avatar = member.Avatar,
            HoTen = member.HoTen,
            GioiTinh = member.GioiTinh,
            NgaySinh = member.NgaySinh,
            NoiSinh = member.NoiSinh,
            NgayMat = member.NgayMat,
            NoiMat = member.NoiMat,
            DoiThu = 0, // TODO: Calculate from relationships
            TieuSu = member.TieuSu,
            TrangThai = member.TrangThai,
            HoId = member.HoId ?? Guid.Empty
        };

        _logger.LogInformation("✅ [GetThanhVienById] Found member: {Id}", request.Id);
        
        return Result<ThanhVienResponse>.Success(response);
    }
}
