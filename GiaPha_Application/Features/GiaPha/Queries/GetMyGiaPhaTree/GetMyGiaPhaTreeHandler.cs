using GiaPha_Application.Common;
using GiaPha_Application.DTOs;
using GiaPha_Application.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GiaPha_Application.Features.GiaPha.Queries.GetMyGiaPhaTree;

public class GetMyGiaPhaTreeHandler : IRequestHandler<GetMyGiaPhaTreeQuery, Result<GiaPhaTreeResponse>>
{
    private readonly IAuthRepository _authRepository;
    private readonly IGiaPhaRepository _giaPhaRepository;
    private readonly ILogger<GetMyGiaPhaTreeHandler> _logger;

    public GetMyGiaPhaTreeHandler(
        IAuthRepository authRepository, 
        IGiaPhaRepository giaPhaRepository,
        ILogger<GetMyGiaPhaTreeHandler> logger)
    {
        _authRepository = authRepository;
        _giaPhaRepository = giaPhaRepository;
        _logger = logger;
    }

    public async Task<Result<GiaPhaTreeResponse>> Handle(GetMyGiaPhaTreeQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("🔍 [GetMyGiaPhaTree] Lấy gia phả cho user: {UserId}", request.UserId);

        // Lấy thông tin user
        var userResult = await _authRepository.GetUserByIdAsync(request.UserId);
        
        if ( userResult == null)
        {
            _logger.LogWarning("⚠️ [GetMyGiaPhaTree] Không tìm thấy user: {UserId}", request.UserId);
            return Result<GiaPhaTreeResponse>.Failure(ErrorType.NotFound, "Không tìm thấy người dùng");
        }

        var user = userResult;

        // Kiểm tra user có thuộc Ho được yêu cầu không
        var isMemberOfHo = user.TaiKhoan_Hos
            .Any(th => th.HoId == request.HoId);
        
        if (!isMemberOfHo)
        {
            _logger.LogWarning("⚠️ [GetMyGiaPhaTree] User {UserId} không thuộc Ho {HoId}", request.UserId, request.HoId);
            return Result<GiaPhaTreeResponse>.Failure(ErrorType.Forbidden, "Bạn không có quyền xem gia phả của họ này");
        }
     
        // Lấy gia phả
        var treeResult = await _giaPhaRepository.BuildGiaPhaTreeAsync(
            request.HoId,
            request.MaxLevel, 
            request.IncludeNuGioi,
            request.IncludeDeleted);

        if (!treeResult.IsSuccess)
        {
            _logger.LogError("❌ [GetMyGiaPhaTree] Lỗi khi build gia phả: {ErrorMessage}", treeResult.ErrorMessage);
            return Result<GiaPhaTreeResponse>.Failure(ErrorType.InternalServerError, treeResult.ErrorMessage!);
        }

        _logger.LogInformation(" [GetMyGiaPhaTree] Lấy gia phả thành công cho họ: {HoId}", request.HoId);
        
        return treeResult;
    }
}
