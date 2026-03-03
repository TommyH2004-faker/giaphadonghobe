using FluentValidation;

namespace GiaPha_Application.Features.Auth.Command.ForgetPassword;
public class ForgetpasswordCValidation :AbstractValidator<ForgetPasswordCommand>
{
    public ForgetpasswordCValidation()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email không được để trống")
            .EmailAddress().WithMessage("Email không hợp lệ");
    }
}