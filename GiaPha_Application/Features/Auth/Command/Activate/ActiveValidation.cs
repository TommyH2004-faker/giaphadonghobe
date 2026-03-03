using FluentValidation;

namespace GiaPha_Application.Features.Auth.Command.Activate;
public class ActiveValidation:AbstractValidator<ActivateUserCommand>
{
    public ActiveValidation()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId không được để trống");
        RuleFor(x => x.ActivationCode).NotEmpty().WithMessage("Mã kích hoạt không được để trống");
    }
}