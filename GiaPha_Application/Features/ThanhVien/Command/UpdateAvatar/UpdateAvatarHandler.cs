using GiaPha_Application.Common;
using GiaPha_Application.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GiaPha_Application.Features.ThanhVien.Command.UpdateAvatar;

public class UpdateAvatarHandler : IRequestHandler<UpdateAvatarCommand, Result<string>>
{
    private readonly IThanhVienRepository _thanhVienRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateAvatarHandler> _logger;

    public UpdateAvatarHandler(
        IThanhVienRepository thanhVienRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateAvatarHandler> logger)
    {
        _thanhVienRepository = thanhVienRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<string>> Handle(UpdateAvatarCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("📸 [UpdateAvatar] Updating avatar for member: {Id}", request.ThanhVienId);

        // Get member
        var memberResult = await _thanhVienRepository.GetThanhVienByIdAsync(request.ThanhVienId);
        
        if (!memberResult.IsSuccess || memberResult.Data == null)
        {
            _logger.LogWarning("⚠️ [UpdateAvatar] Member not found: {Id}", request.ThanhVienId);
            return Result<string>.Failure(ErrorType.NotFound, "Không tìm thấy thành viên");
        }

        var member = memberResult.Data;

        // Update avatar
        member.UpdateAvatar(request.AvatarUrl);

        var updateResult = await _thanhVienRepository.UpdateThanhVienAsync(member);
        if (!updateResult.IsSuccess)
        {
            return Result<string>.Failure(updateResult.ErrorType ?? ErrorType.InternalServerError, updateResult.ErrorMessage ?? "Cập nhật avatar thất bại");
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("✅ [UpdateAvatar] Avatar updated successfully: {Id}", request.ThanhVienId);

        return Result<string>.Success(request.AvatarUrl);
    }
}
