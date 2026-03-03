using GiaPha_Application.Common;
using GiaPha_Application.Repository;
using MediatR;

namespace GiaPha_Application.Features.TaiKhoanNguoiDungs.Command.UpdateAvatar;
public class UpdateAvatarHandle : IRequestHandler<UpdateAvatarCommand, Result<bool>>
{
    private readonly IAuthRepository authRepository;

    public UpdateAvatarHandle(IAuthRepository authRepository)
    {
        this.authRepository = authRepository;
    }

    public async Task<Result<bool>> Handle(UpdateAvatarCommand request, CancellationToken cancellationToken)
    {
        var taiKhoan = await authRepository.GetUserByIdAsync(request.Id);
        if (taiKhoan == null) return Result<bool>.Failure(ErrorType.NotFound, "Không tìm thấy tài khoản");

        taiKhoan.UpdateAvatar(request.Avatar);
        var result = await authRepository.UpdateUserAsync(taiKhoan);
        if (result == null ) return Result<bool>.Failure(ErrorType.InternalError, "Cập nhật avatar thất bại");

        return Result<bool>.Success(true);
    }
}