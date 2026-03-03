using FluentValidation;

namespace GiaPha_Application.Features.Auth.Command.Login;
public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.TenDangNhap).NotEmpty().WithMessage("Tên đăng nhập không được để trống");
        RuleFor(x => x.MatKhau).NotEmpty().WithMessage("Mật khẩu không được để trống");
    }
}