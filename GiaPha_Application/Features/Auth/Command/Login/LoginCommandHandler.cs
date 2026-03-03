using GiaPha_Application.Common;
using GiaPha_Application.DTOs;
using GiaPha_Application.Repository;
using GiaPha_Application.Service;
using GiaPha_Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace GiaPha_Application.Features.Auth.Command.Login;
public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginRespone>>
{
    private readonly IAuthRepository _authRepository;
    private readonly IJwtService _jwtService;
    private readonly IHoRepository _hoRepository;

    public LoginCommandHandler(
        IAuthRepository authRepository, 
        IJwtService jwtService,
        IHoRepository hoRepository)
    {
        _authRepository = authRepository;
        _jwtService = jwtService;
        _hoRepository = hoRepository;
    }
    public async Task<Result<LoginRespone>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // user theo username
        var userResult = await _authRepository.GetUserByUsernameAsync(request.TenDangNhap);
        if (userResult == null)
        {
            return Result<LoginRespone>.Failure(ErrorType.NotFound,"ACCOUNT_NOT_FOUND"); // Mã lỗi cụ thể
        }

        var user = userResult;

        //Kiểm tra tài khoản có được kích hoạt chưa TRƯỚC KHI kiểm tra password
        if (!user.Enabled)
        {
            return Result<LoginRespone>.Failure(ErrorType.NotActivated,"ACCOUNT_NOT_ACTIVATED"); // Mã lỗi cụ thể
        }

        // 3. Kiểm tra password SAU khi đã biết tài khoản đã kích hoạt
        var isValidPassword = BCrypt.Net.BCrypt.Verify(request.MatKhau, user.MatKhau);
        if (!isValidPassword)
        {
            return Result<LoginRespone>.Failure(ErrorType.WrongPassword,"WRONG_PASSWORD"); // Mã lỗi cụ thể
        }

        // 4. Lấy tất cả các Ho mà user thuộc về (với role)
        var availableHos = user.TaiKhoan_Hos
            .Select(th => new HoWithRoleResponse
            {
                Id = th.Ho.Id,
                TenHo = th.Ho.TenHo,
                MoTa = th.Ho.MoTa,
                HinhAnh = th.Ho.HinhAnh,
                QueQuan = th.Ho.QueQuan,
                ThuyToId = th.Ho.ThuyToId,
                RoleInHo = (int)th.RoleInHo
            })
            .ToList();

        // 5. Lấy Ho đầu tiên làm Ho mặc định (ưu tiên Trưởng họ)
        var currentHo = user.TaiKhoan_Hos
            .OrderBy(th => (int)th.RoleInHo) // TruongHo (0) trước ThanhVien (1)
            .FirstOrDefault();
        
        var selectedHoId = currentHo?.HoId;
        var roleInCurrentHo = currentHo != null ? (int)currentHo.RoleInHo : (int?)null;

        // 6. Tạo JWT token với HoId và RoleInHo mặc định
        var token = _jwtService.GenerateToken(user, selectedHoId, roleInCurrentHo);

        var response = new LoginRespone
        {
            Token = token,
            Email = user.Email,
            TenDangNhap = user.TenDangNhap,
            Role = user.Role,
            AvailableHos = availableHos,
            SelectedHoId = selectedHoId,
            RoleInCurrentHo = roleInCurrentHo
        };
        return Result<LoginRespone>.Success(response);
    }
}