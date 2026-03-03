using GiaPha_Application.Common;
using GiaPha_Application.DTOs;
using GiaPha_Application.Repository;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GiaPha_Application.Features.Auth.Command.Changepassword.ChangePasswordCommand;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand,Result<bool>>
{
    private readonly IAuthRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ChangePasswordCommandHandler(IAuthRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        // Lấy user từ database
        var user = await _userRepository.GetUserByIdAsync(request.UserId);
        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }

        // Kiểm tra user đã kích hoạt chưa
        if (user == null || user.Enabled == false)
        {
            throw new InvalidOperationException("Account not activated");
        }

        // Xác thực mật khẩu cũ
        var isOldPasswordValid = BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.MatKhau);
        if (!isOldPasswordValid)
        {
            throw new InvalidOperationException("Old password is incorrect");
        }

        // Hash mật khẩu mới
        var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

        // Thay đổi mật khẩu (domain method sẽ tự động raise event)
        user.ChangePassword(newPasswordHash);

        // Lưu thay đổi và dispatch events
        await _userRepository.UpdateUserAsync(user);
        await _unitOfWork.SaveChangesAsync(); //  Events được dispatch tự động

        return Result<bool>.Success(true);
    }
}