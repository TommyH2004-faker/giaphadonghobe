using FluentValidation;

namespace GiaPha_Application.Features.Notification.Commands.CreateNotification;
public class CreateNotificationValidation : AbstractValidator<CreateNotificationCommand>
{
    public CreateNotificationValidation()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId không được để trống");

        RuleFor(x => x.HoId)
            .NotEmpty().WithMessage("HoId không được để trống");

        RuleFor(x => x.NoiDung)
            .NotEmpty().WithMessage("Nội dung thông báo không được để trống")
            .MaximumLength(1000).WithMessage("Nội dung không được vượt quá 1000 ký tự");
    }
}