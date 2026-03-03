using GiaPha_Application.Common;
using GiaPha_Application.DTOs;
using GiaPha_Application.Repository;
using GiaPha_Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GiaPha_Application.Features.HoName.Command.CreateHo;

public class CreateHoHandle : IRequestHandler<CreateHoCommand, Result<HoResponse>>
{
    private readonly IHoRepository _hoRepository;
    private readonly IThanhVienRepository _thanhVienRepository;
    private readonly IAuthRepository _authRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateHoHandle> _logger;

    public CreateHoHandle(
        IHoRepository hoRepository,

        IThanhVienRepository thanhVienRepository,
        IAuthRepository authRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateHoHandle> logger)
    {
        _hoRepository = hoRepository;
        _thanhVienRepository = thanhVienRepository;
        _authRepository = authRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<HoResponse>> Handle(CreateHoCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(" Bắt đầu tạo Họ mới - UserId: {UserId}, TenHo: {TenHo}, NgaySinhThuyTo: {NgaySinhThuyTo}", 
                request.UserId, request.TenHo, request.NgaySinhThuyTo);

            //  Kiểm tra User có tồn tại không
            _logger.LogInformation(" Kiểm tra User tồn tại...");
            var userResult = await _authRepository.GetUserByIdAsync(request.UserId);
            if (userResult == null )
            {
                _logger.LogError(" Người dùng không tồn tại: {UserId}", request.UserId);
                return Result<HoResponse>.Failure(ErrorType.NotFound, "Người dùng không tồn tại");
            }

            var user = userResult;
            _logger.LogInformation(" User tồn tại: {Email}", user.Email);

            //  Kiểm tra User đã thuộc họ nào chưa (1 user chỉ được ở 1 họ)
            _logger.LogInformation(" Kiểm tra User có thuộc họ nào không...");
            if (user.TaiKhoan_Hos.Any())
            {
                _logger.LogWarning(" User đã thuộc họ khác");
                return Result<HoResponse>.Failure(ErrorType.Conflict, "Bạn đã thuộc một họ. Mỗi người chỉ được thuộc 1 họ duy nhất");
            }

            //  Kiểm tra tên họ đã tồn tại chưa
            _logger.LogInformation(" Kiểm tra tên họ...");
            var existingHo = await _hoRepository.GetHoByNameAsync(request.TenHo);
            if (existingHo.Data != null)
            {
                _logger.LogWarning(" Họ đã tồn tại: {TenHo}", request.TenHo);
                return Result<HoResponse>.Failure(ErrorType.Conflict, "Họ đã tồn tại. Vui lòng chọn tên khác hoặc xin tham gia họ này");
            }

            //  Tạo Họ mới
            _logger.LogInformation(" Tạo Họ mới...");
            var hoResult = await _hoRepository.CreateHoAsync(request.TenHo, request.MoTa, request.QueQuan);
        if (!hoResult.IsSuccess || hoResult.Data == null)
        {
            return Result<HoResponse>.Failure(ErrorType.Failure, "Tạo Họ thất bại");
        }

        var newHo = hoResult.Data;
        _logger.LogInformation(" Đã tạo Họ: {HoId} - {TenHo}", newHo.Id, newHo.TenHo);

       
        // Tạo Thành viên Thủy Tổ
        _logger.LogInformation(" Tạo Thành viên Thủy Tổ: {HoTen}, NgaySinh: {NgaySinh}...", 
            request.HoTenThuyTo, request.NgaySinhThuyTo);
        var thanhVien = GiaPha_Domain.Entities.ThanhVien.Create(
            request.HoTenThuyTo,
            request.GioiTinhThuyTo,
            request.NgaySinhThuyTo,
            request.NoiSinhThuyTo ?? "",
            request.TieuSuThuyTo,
            true, // TrangThai = true
            newHo.Id
        );

        var thanhVienResult = await _thanhVienRepository.CreateThanhVienAsync(thanhVien);
        if (!thanhVienResult.IsSuccess || thanhVienResult.Data == null)
        {
            _logger.LogError(" Không thể tạo Thành viên Thủy Tổ");
            return Result<HoResponse>.Failure(ErrorType.Failure, "Không thể tạo Thành viên Thủy Tổ");
        }

        _logger.LogInformation(" Đã tạo Thành viên Thủy Tổ: {ThanhVienId}", thanhVien.Id);

        //  Link User với Họ (RoleInHo = 0 - Trưởng họ)
        //  QUAN TRỌNG: KHÔNG set ThuyToId trước SaveChanges để tránh FK constraint error
        _logger.LogInformation(" Link User với Họ...");
        var addUserResult = await _authRepository.AddUserToHoAsync(
            request.UserId,
            newHo.Id,
            RoleCuaHo.TruongHo // 0 = Trưởng họ
        );

        if (addUserResult==null)
        {
            _logger.LogError(" Không thể link User với Họ");
            return Result<HoResponse>.Failure(ErrorType.Failure, "Không thể link User với Họ");
        }

        _logger.LogInformation(" Đã link User {UserId} với Họ {HoId} (Trưởng họ)", request.UserId, newHo.Id);

        //  Lưu vào database (Chưa set ThuyToId)
        _logger.LogInformation(" Lưu vào database lần 1...");
        
        try
        {
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("✅ Đã lưu Họ, ThanhVien, TaiKhoan_Ho");
        }
        catch (Exception saveEx)
        {
            _logger.LogError(saveEx, " Lỗi SaveChanges: {Message}, InnerException: {Inner}", 
                saveEx.Message, saveEx.InnerException?.Message);
            throw;
        }

        // Sau khi insert thành công, giờ mới set ThuyToId
        _logger.LogInformation("9️ Set Thủy Tổ cho Họ...");
        newHo.ThuyToId = thanhVien.Id;
        
        try
        {
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation(" Đã update ThuyToId cho Họ");
        }
        catch (Exception updateEx)
        {
            _logger.LogError(updateEx, " Lỗi khi update ThuyToId: {Message}", updateEx.Message);
            // Không throw vì đã tạo Ho thành công, chỉ là chưa set ThuyToId
        }

        _logger.LogInformation(" Tạo họ mới thành công!");

        return Result<HoResponse>.Success(new HoResponse
        {
            Id = newHo.Id,
            TenHo = newHo.TenHo,
            MoTa = newHo.MoTa,
            QueQuan = newHo.QueQuan,
            ThuyToId = newHo.ThuyToId
        });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, " LỖI khi tạo Họ: {Message}", ex.Message);
            _logger.LogError(" StackTrace: {StackTrace}", ex.StackTrace);
            _logger.LogError(" InnerException: {InnerException}", ex.InnerException?.Message);
            return Result<HoResponse>.Failure(ErrorType.Failure, $"Lỗi khi tạo Họ: {ex.Message}");
        }
    }
}