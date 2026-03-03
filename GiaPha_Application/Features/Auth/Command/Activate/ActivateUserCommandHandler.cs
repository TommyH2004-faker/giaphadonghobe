using GiaPha_Application.Common;
using GiaPha_Application.Repository;
using MediatR;

namespace GiaPha_Application.Features.Auth.Command.Activate;
public class ActivateUserCommandHandler : IRequestHandler<ActivateUserCommand, Result<bool>>
{
    private readonly IAuthRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActivateUserCommandHandler(IAuthRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(ActivateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByIdAsync(request.UserId);
        if (user == null)
        {
            return Result<bool>.Failure(ErrorType.NotFound,"Người dùng không tồn tại.");
        }

        if (user.Enabled)
        {
            return Result<bool>.Failure(ErrorType.Conflict,"Tài khoản đã được kích hoạt trước đó.");
        }

        if (user.ActivationCode != request.ActivationCode)
        {
            return Result<bool>.Failure(ErrorType.Unauthorized,"Mã kích hoạt không hợp lệ.");
        }
        user.Activate();

        await _userRepository.UpdateUserAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Success(true);
    }
}