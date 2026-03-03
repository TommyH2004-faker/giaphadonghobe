using GiaPha_Application.Common;
using GiaPha_Application.Repository;
using MediatR;

namespace GiaPha_Application.Features.Auth.Command.ForgetPassword;

public class ForgetPasswordHandle : IRequestHandler<ForgetPasswordCommand, Result<bool>>
{
    private readonly IAuthRepository _authRepository;
    private readonly IUnitOfWork _unitOfWork;
    public ForgetPasswordHandle(IAuthRepository authRepository, IUnitOfWork unitOfWork)
    {
        _authRepository = authRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<Result<bool>> Handle(ForgetPasswordCommand request, CancellationToken cancellationToken)
    {   
        var userResult = await _authRepository.GetUserByEmailAsync(request.Email);
    if (userResult == null)
    {
        return Result<bool>.Failure(ErrorType.NotFound, "Người dùng không tồn tại.");
    }

    // 1. Sinh mật khẩu mới
    var plainPassword = GenerateRandomPassword(8);

    // 2. Băm mật khẩu
    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword);

    // 3. Lưu mật khẩu mới (domain method sẽ tự động raise event)
    userResult.ForgotPassword(hashedPassword, plainPassword);

    await _authRepository.UpdateUserAsync(userResult);
    await _unitOfWork.SaveChangesAsync(); // Events được dispatch tự động

    return Result<bool>.Success(true);
    }
    private string GenerateRandomPassword(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}