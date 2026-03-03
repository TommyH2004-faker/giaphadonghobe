using GiaPha_Application.Common;
using GiaPha_Application.Repository;
using GiaPha_Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GiaPha_Application.Features.YeuCau.Commands.TuChoiYeuCau;

public class TuChoiYeuCauHandler : IRequestHandler<TuChoiYeuCauCommand, Result<bool>>
{
    private readonly IYeuCauThamGiaHoRepository _yeuCauRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TuChoiYeuCauHandler> _logger;

    public TuChoiYeuCauHandler(
        IYeuCauThamGiaHoRepository yeuCauRepo,
        IUnitOfWork unitOfWork,
        ILogger<TuChoiYeuCauHandler> logger)
    {
        _yeuCauRepo = yeuCauRepo;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(TuChoiYeuCauCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var yeuCau = await _yeuCauRepo.GetByIdAsync(request.YeuCauId);
            if (yeuCau == null)
                return Result<bool>.Failure(ErrorType.NotFound, "Không tìm thấy yêu cầu");

            if (yeuCau.TrangThai != TrangThaiYeuCau.DangCho)
                return Result<bool>.Failure(ErrorType.Conflict, "Yêu cầu này đã được xử lý");

            yeuCau.TuChoi(request.NguoiXuLyId, request.GhiChu);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("❌ Từ chối yêu cầu {YeuCauId}", request.YeuCauId);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Lỗi khi từ chối yêu cầu {YeuCauId}", request.YeuCauId);
            return Result<bool>.Failure(ErrorType.InternalServerError, "Lỗi khi từ chối yêu cầu");
        }
    }
}
