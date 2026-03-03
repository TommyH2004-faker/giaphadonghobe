using GiaPha_Application.Common;
using GiaPha_Application.DTOs;
using GiaPha_Application.Repository;
using MediatR;

namespace GiaPha_Application.Features.HonNhan.Command.Create;

public class CreateHonNhanHandle : IRequestHandler<CreateHonNhanCommand, Result<HonNhanResponse>>
{
    private readonly IHonNhanRepository _honNhanRepository;
    private readonly IThanhVienRepository _thanhVienRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateHonNhanHandle(
        IHonNhanRepository honNhanRepository,
        IThanhVienRepository thanhVienRepository,
        IUnitOfWork unitOfWork)
    {
        _honNhanRepository = honNhanRepository;
        _thanhVienRepository = thanhVienRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<HonNhanResponse>> Handle(CreateHonNhanCommand request, CancellationToken cancellationToken)
    {
        // 1. Không cùng người
        if (request.ChongId == request.VoId)
            return Result<HonNhanResponse>.Failure(ErrorType.Validation, "Không thể tạo hôn nhân với chính mình.");

        // 2. Kiểm tra tồn tại thành viên
        var chong = await _thanhVienRepository.GetThanhVienByIdAsync(request.ChongId);
        var vo = await _thanhVienRepository.GetThanhVienByIdAsync(request.VoId);

        if (chong == null || chong.Data == null)
            return Result<HonNhanResponse>.Failure(ErrorType.Validation, "Chồng không tồn tại.");

        if (vo == null || vo.Data == null)
            return Result<HonNhanResponse>.Failure(ErrorType.Validation, "Vợ không tồn tại.");

        // Kiểm tra giới tính
        if (chong.Data.GioiTinh != false) // false: Nam
            return Result<HonNhanResponse>.Failure(ErrorType.Validation, "Chồng phải là nam giới.");

        if (vo.Data.GioiTinh != true) // true: Nữ
            return Result<HonNhanResponse>.Failure(ErrorType.Validation, "Vợ phải là nữ giới.");

        // 3. Không trùng cặp
        var existed = await _honNhanRepository.ExistsAsync(request.ChongId, request.VoId);
        if (existed)
            return Result<HonNhanResponse>.Failure(ErrorType.Validation, "Cặp hôn nhân này đã tồn tại.");

        // 4. Không có hôn nhân active khác
        var chongHasActiveMarriage = await _honNhanRepository.HasActiveMarriageAsync(request.ChongId);
        if (chongHasActiveMarriage)
            return Result<HonNhanResponse>.Failure(ErrorType.Validation, "Chồng đã có hôn nhân đang hoạt động.");

        var voHasActiveMarriage = await _honNhanRepository.HasActiveMarriageAsync(request.VoId);
        if (voHasActiveMarriage)
            return Result<HonNhanResponse>.Failure(ErrorType.Validation, "Vợ đã có hôn nhân đang hoạt động.");

        // 5. Không huyết thống
        // var isBloodRelated = await _honNhanRepository.IsBloodRelatedAsync(request.ChongId, request.VoId);
        // if (isBloodRelated)
        //     return Result<HonNhanResponse>.Failure(ErrorType.Validation, "Không thể kết hôn với người có quan hệ huyết thống.");
       
        // 5. Không huyết thống (kiểm tra cùng họ)
        if (chong.Data.HoId == vo.Data.HoId)
            return Result<HonNhanResponse>.Failure(ErrorType.Validation, "Không thể kết hôn với người cùng họ (huyết thống).");

        // Nếu muốn kiểm tra chi tiết hơn
        var isBloodRelated = await _honNhanRepository.IsBloodRelatedAsync(request.ChongId, request.VoId);
        if (isBloodRelated)
            return Result<HonNhanResponse>.Failure(ErrorType.Validation, "Không thể kết hôn với người có quan hệ huyết thống.");
        // Tạo hôn nhân
        var honNhan = GiaPha_Domain.Entities.HonNhan.Create(
            request.ChongId, 
            request.VoId, 
            request.NgayKetHon);

        var created = await _honNhanRepository.CreateAsync(honNhan);
        await _unitOfWork.SaveChangesAsync();

        if (created.Data == null)
            return Result<HonNhanResponse>.Failure(ErrorType.Validation, "Tạo hôn nhân thất bại.");

        var response = new HonNhanResponse
        {
            Id = created.Data.Id,
            ChongId = created.Data.ChongId,
            VoId = created.Data.VoId,
            NgayKetHon = created.Data.NgayKetHon ?? DateTime.UtcNow,
            NgayLyHon = created.Data.NgayLyHon,
            TrangThai = created.Data.TrangThai
        };

        return Result<HonNhanResponse>.Success(response);
    }
}