using GiaPha_Application.Common;
using GiaPha_Application.DTOs;
using GiaPha_Application.Repository;
using MediatR;
using GiaPha_Domain.Entities;
namespace GiaPha_Application.Features.ThanhVien.Command.Create;
public class CreateThanhVienHandle : IRequestHandler<CreateThanhVienCommand, Result<ThanhVienResponse>>
{
    private readonly IThanhVienRepository _thanhVienRepository;
    private readonly IHoRepository _hoRepository;
    private readonly IHonNhanRepository _honNhanRepository;
    private readonly IQuanHeChaMeRepository _quanHeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateThanhVienHandle(
        IThanhVienRepository thanhVienRepository, 
        IHoRepository hoRepository, 
        IHonNhanRepository honNhanRepository,
        IQuanHeChaMeRepository quanHeRepository,
        IUnitOfWork unitOfWork)
    {
        _thanhVienRepository = thanhVienRepository;
        _hoRepository = hoRepository;
        _honNhanRepository = honNhanRepository;
        _quanHeRepository = quanHeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ThanhVienResponse>> Handle(CreateThanhVienCommand request, CancellationToken cancellationToken)
    {
        // Validate họ chỉ khi HoId được cung cấp (vợ/chồng từ họ khác có thể không có HoId)
        if (request.HoId.HasValue && request.HoId.Value != Guid.Empty)
        {
            var ho = await _hoRepository.GetHoByIdAsync(request.HoId.Value);
            if (ho == null || ho.Data == null)
            {
                return Result<ThanhVienResponse>.Failure(ErrorType.Validation, "Họ không tồn tại");
            }
        }

        // Nếu có ParentId, validate parent
        if (request.ParentId.HasValue)
        {
            var parentResult = await _thanhVienRepository.GetThanhVienByIdAsync(request.ParentId.Value);
            if (parentResult == null || !parentResult.IsSuccess || parentResult.Data == null)
            {
                return Result<ThanhVienResponse>.Failure(ErrorType.Validation, "Cha/Mẹ không tồn tại");
            }

            var parent = parentResult.Data;

            // Validate parent thuộc cùng họ (chỉ khi request có HoId)
            if (request.HoId.HasValue && request.HoId.Value != Guid.Empty && parent.HoId != request.HoId.Value)
            {
                return Result<ThanhVienResponse>.Failure(ErrorType.Validation, "Cha/Mẹ phải thuộc cùng họ");
            }
        }
    
        var thanhVien = GiaPha_Domain.Entities.ThanhVien.Create(
            request.HoTen,
            request.GioiTinh,
            request.NgaySinh,
            request.NoiSinh,
            request.TieuSu,
            request.TrangThai,
            request.HoId
        );

        var createdThanhVien = await _thanhVienRepository.CreateThanhVienAsync(thanhVien);
        
        if (createdThanhVien.Data == null)
        {
            return Result<ThanhVienResponse>.Failure(ErrorType.Validation, "Tạo thành viên thất bại, vui lòng kiểm tra lại thông tin");
        }

        // Tạo quan hệ cha-con nếu có ParentId
        // if (request.ParentId.HasValue)
        // {
        //     var parentResult = await _thanhVienRepository.GetThanhVienByIdAsync(request.ParentId.Value);
        //     if (parentResult.IsSuccess && parentResult.Data != null)
        //     {
        //         var parent = parentResult.Data;
                
        //         // Xác định loại quan hệ: 0 = Cha (Nam), 1 = Mẹ (Nữ)
        //         int loaiQuanHe = parent.GioiTinh ? 1 : 0;
                
        //         var quanHe = QuanHeChaCon.Create(
        //             chaMeId: request.ParentId.Value,
        //             conId: createdThanhVien.Data.Id,
        //             loai: loaiQuanHe
        //         );

        //         await _quanHeRepository.CreateAsync(quanHe);
        //     }
        // }
        // Tạo quan hệ cha-con nếu có ParentId
if (request.ParentId.HasValue)
{
    var parentResult = await _thanhVienRepository.GetThanhVienByIdAsync(request.ParentId.Value);

    if (parentResult.IsSuccess && parentResult.Data != null)
    {
        var parent = parentResult.Data;

        int loaiQuanHe = parent.GioiTinh ? 1 : 0;

        // 1️⃣ Tạo quan hệ cho parent
        var quanHe = QuanHeChaCon.Create(
            parent.Id,
            createdThanhVien.Data.Id,
            loaiQuanHe
        );

        await _quanHeRepository.CreateAsync(quanHe);

        // 2️⃣ Tìm hôn nhân đang hoạt động
        var honNhan = await _honNhanRepository.GetActiveHonNhanByMemberId(parent.Id);

        if (honNhan != null && honNhan.Data != null)
        {
            Guid spouseId = honNhan.Data.ChongId == parent.Id
                ? honNhan.Data.VoId
                : honNhan.Data.ChongId;

            var spouseResult = await _thanhVienRepository.GetThanhVienByIdAsync(spouseId);

            if (spouseResult.IsSuccess && spouseResult.Data != null)
            {
                var spouse = spouseResult.Data;

                int spouseLoai = spouse.GioiTinh ? 1 : 0;

                var spouseQuanHe = QuanHeChaCon.Create(
                    spouse.Id,
                    createdThanhVien.Data.Id,
                    spouseLoai
                );

                await _quanHeRepository.CreateAsync(spouseQuanHe);
            }
        }
    }
}

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var thanhVienResponse = new ThanhVienResponse
        {
            Id = createdThanhVien.Data.Id,
            HoTen = createdThanhVien.Data.HoTen,
            GioiTinh = createdThanhVien.Data.GioiTinh,
            NgaySinh = createdThanhVien.Data.NgaySinh,
            NoiSinh = createdThanhVien.Data.NoiSinh!,
            TieuSu = createdThanhVien.Data.TieuSu,
            TrangThai = createdThanhVien.Data.TrangThai,
            HoId = createdThanhVien.Data.HoId ?? Guid.Empty
        };

        return Result<ThanhVienResponse>.Success(thanhVienResponse);
    }
} 