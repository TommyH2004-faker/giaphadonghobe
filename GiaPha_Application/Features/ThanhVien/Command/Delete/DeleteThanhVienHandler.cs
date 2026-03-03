// using GiaPha_Application.Common;
// using GiaPha_Application.Repository;
// using MediatR;
// using Microsoft.Extensions.Logging;

// namespace GiaPha_Application.Features.ThanhVien.Command.Delete;

// public class DeleteThanhVienHandler : IRequestHandler<DeleteThanhVienCommand, Result<bool>>
// {
//     private readonly IThanhVienRepository _thanhVienRepository;
//     private readonly IUnitOfWork _unitOfWork;
//     private readonly ILogger<DeleteThanhVienHandler> _logger;

//     public DeleteThanhVienHandler(
//         IThanhVienRepository thanhVienRepository,
//         IUnitOfWork unitOfWork,
//         ILogger<DeleteThanhVienHandler> logger)
//     {
//         _thanhVienRepository = thanhVienRepository;
//         _unitOfWork = unitOfWork;
//         _logger = logger;
//     }

//     public async Task<Result<bool>> Handle(DeleteThanhVienCommand request, CancellationToken cancellationToken)
//     {
//         _logger.LogInformation("🗑️ [DeleteThanhVien] Deleting member: {Id}", request.Id);

//         // Get member
//         var memberResult = await _thanhVienRepository.GetThanhVienByIdAsync(request.Id);
        
//         if (!memberResult.IsSuccess || memberResult.Data == null)
//         {
//             _logger.LogWarning("⚠️ [DeleteThanhVien] Member not found: {Id}", request.Id);
//             return Result<bool>.Failure(ErrorType.NotFound, "Không tìm thấy thành viên");
//         }

//         var member = memberResult.Data;

//         // Delete member
//         var deleteResult = await _thanhVienRepository.DeleteThanhVienAsync(member.Id);
//         if (!deleteResult.IsSuccess)
//         {
//             return Result<bool>.Failure(deleteResult.ErrorType ?? ErrorType.InternalServerError, deleteResult.ErrorMessage ?? "Xóa thất bại");
//         }
        
//         await _unitOfWork.SaveChangesAsync(cancellationToken);

//         _logger.LogInformation("✅ [DeleteThanhVien] Deleted successfully: {Id}", request.Id);
        
//         return Result<bool>.Success(true);
//     }
// }
using GiaPha_Application.Common;
using GiaPha_Application.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GiaPha_Application.Features.ThanhVien.Command.Delete;

public class DeleteThanhVienHandler : IRequestHandler<DeleteThanhVienCommand, Result<bool>>
{
    private readonly IThanhVienRepository _thanhVienRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteThanhVienHandler> _logger;

    public DeleteThanhVienHandler(
        IThanhVienRepository thanhVienRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteThanhVienHandler> logger)
    {
        _thanhVienRepository = thanhVienRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(DeleteThanhVienCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("🗑️ [DeleteThanhVien] CASCADE Soft deleting member: {Id}", request.Id);

        var memberResult = await _thanhVienRepository.GetThanhVienByIdAsync(request.Id);

        if (!memberResult.IsSuccess || memberResult.Data == null)
        {
            _logger.LogWarning("⚠️ [DeleteThanhVien] Member not found: {Id}", request.Id);
            return Result<bool>.Failure(ErrorType.NotFound, "Không tìm thấy thành viên");
        }

        var member = memberResult.Data;

        // ✅ CASCADE SOFT DELETE: Xóa người này + TẤT CẢ con cháu phía sau
        var deleteResult = await _thanhVienRepository.CascadeDeleteThanhVienAsync(member.Id);

        if (!deleteResult.IsSuccess)
        {
            _logger.LogError("❌ [DeleteThanhVien] Failed to delete: {ErrorMessage}", deleteResult.ErrorMessage);
            return Result<bool>.Failure(
                deleteResult.ErrorType ?? ErrorType.InternalServerError,
                deleteResult.ErrorMessage ?? "Xóa thất bại"
            );
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("✅ [DeleteThanhVien] CASCADE Soft deleted successfully: {Id} and {Count} descendants", 
            request.Id, deleteResult.Data);

        return Result<bool>.Success(true);
    }
}