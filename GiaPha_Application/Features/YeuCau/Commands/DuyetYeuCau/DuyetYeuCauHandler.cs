using GiaPha_Application.Common;
using GiaPha_Application.Repository;
using GiaPha_Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GiaPha_Application.Features.YeuCau.Commands.DuyetYeuCau;

public class DuyetYeuCauHandler : IRequestHandler<DuyetYeuCauCommand, Result<bool>>
{
    private readonly IYeuCauThamGiaHoRepository _yeuCauRepo;
    private readonly IAuthRepository _authRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DuyetYeuCauHandler> _logger;

    public DuyetYeuCauHandler(
        IYeuCauThamGiaHoRepository yeuCauRepo,
        IAuthRepository authRepository,
        IUnitOfWork unitOfWork,
        ILogger<DuyetYeuCauHandler> logger)
    {
        _yeuCauRepo = yeuCauRepo;
        _authRepository = authRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(DuyetYeuCauCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Lấy yêu cầu
            var yeuCau = await _yeuCauRepo.GetByIdAsync(request.YeuCauId);
            if (yeuCau == null)
                return Result<bool>.Failure(ErrorType.NotFound, "Không tìm thấy yêu cầu");

            if (yeuCau.TrangThai != TrangThaiYeuCau.DangCho)
                return Result<bool>.Failure(ErrorType.Conflict, "Yêu cầu này đã được xử lý");

            // 2. Duyệt yêu cầu
            yeuCau.Duyet(request.NguoiXuLyId);

            // 3. Thêm user vào họ với role Thành viên
            await _authRepository.AddUserToHoAsync(yeuCau.UserId, yeuCau.HoId, RoleCuaHo.ThanhVien);

            // 4. Lưu
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("✅ Duyệt yêu cầu {YeuCauId} - User {UserId} vào họ {HoId}",
                request.YeuCauId, yeuCau.UserId, yeuCau.HoId);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Lỗi khi duyệt yêu cầu {YeuCauId}", request.YeuCauId);
            return Result<bool>.Failure(ErrorType.InternalServerError, "Lỗi khi duyệt yêu cầu");
        }
    }
}
