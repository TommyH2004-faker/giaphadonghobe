

using FluentValidation;

namespace GiaPha_Application.Features.Auth.Command.Register;

public class RegisterValidator : AbstractValidator<RegisterCommand>
{
    public RegisterValidator()
    {
        RuleFor(x => x.TenDangNhap)
            .NotEmpty().WithMessage("Tên đăng nhập không được để trống")
            .MaximumLength(50).WithMessage("Tên đăng nhập không được vượt quá 50 ký tự");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email không được để trống")
            .EmailAddress().WithMessage("Email không hợp lệ");

        RuleFor(x => x.MatKhau)
            .NotEmpty().WithMessage("Mật khẩu không được để trống")
            .MinimumLength(6).WithMessage("Mật khẩu phải có ít nhất 6 ký tự");
    }
}
