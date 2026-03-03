using GiaPha_Application.Common;
using GiaPha_Application.DTOs;
using GiaPha_Application.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GiaPha_Application.Features.ThanhVien.Command.Update;

public class UpdateThanhVienHandler : IRequestHandler<UpdateThanhVienCommand, Result<ThanhVienResponse>>
{
    private readonly IThanhVienRepository _thanhVienRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateThanhVienHandler> _logger;

    public UpdateThanhVienHandler(
        IThanhVienRepository thanhVienRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateThanhVienHandler> logger)
    {
        _thanhVienRepository = thanhVienRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<ThanhVienResponse>> Handle(UpdateThanhVienCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("✏️ [UpdateThanhVien] Updating member: {Id}", request.Id);

        // Get member
        var memberResult = await _thanhVienRepository.GetThanhVienByIdAsync(request.Id);
        
        if (!memberResult.IsSuccess || memberResult.Data == null)
        {
            _logger.LogWarning("⚠️ [UpdateThanhVien] Member not found: {Id}", request.Id);
            return Result<ThanhVienResponse>.Failure(ErrorType.NotFound, "Không tìm thấy thành viên");
        }

        var member = memberResult.Data;

        // Update fields
        member.Update(
            hoTen: request.HoTen,
            gioiTinh: request.GioiTinh,
            ngaySinh: request.NgaySinh,
            noiSinh: request.NoiSinh,
            ngayMat: request.NgayMat,
            noiMat: request.NoiMat,
            tieuSu: request.TieuSu,
            trangThai: request.TrangThai
        );

        var updateResult = await _thanhVienRepository.UpdateThanhVienAsync(member);
        if (!updateResult.IsSuccess)
        {
            return Result<ThanhVienResponse>.Failure(updateResult.ErrorType ?? ErrorType.InternalServerError, updateResult.ErrorMessage ?? "Cập nhật thất bại");
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("✅ [UpdateThanhVien] Updated successfully: {Id}", request.Id);

        var updatedMember = updateResult.Data;
        
        if (updatedMember == null)
        {
            return Result<ThanhVienResponse>.Failure(ErrorType.InternalServerError, "Không thể lấy thông tin thành viên sau khi cập nhật");
        }

        // Map to response
        var response = new ThanhVienResponse
        {
            Id = updatedMember.Id,
            Avatar = updatedMember.Avatar,
            HoTen = updatedMember.HoTen,
            GioiTinh = updatedMember.GioiTinh,
            NgaySinh = updatedMember.NgaySinh,
            NoiSinh = updatedMember.NoiSinh,
            NgayMat = updatedMember.NgayMat,
            NoiMat = updatedMember.NoiMat,
            DoiThu = 0, // TODO: Calculate from relationships
            TieuSu = updatedMember.TieuSu,
            TrangThai = updatedMember.TrangThai,
            HoId = updatedMember.HoId ?? Guid.Empty
        };
        
        return Result<ThanhVienResponse>.Success(response);
    }
}
