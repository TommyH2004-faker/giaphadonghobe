using FluentValidation;

namespace GiaPha_Application.Features.HonNhan.Command.Create;

public class CreateHonNhanValidation : AbstractValidator<CreateHonNhanCommand>
{
    public CreateHonNhanValidation()
    {
        RuleFor(x => x.ChongId)
            .NotEmpty().WithMessage("ChongId không được để trống");

        RuleFor(x => x.VoId)
            .NotEmpty().WithMessage("VoId không được để trống");

        RuleFor(x => x.NgayKetHon)
            .NotEmpty().WithMessage("Ngày kết hôn không được để trống")
            .LessThanOrEqualTo(DateTime.Now).WithMessage("Ngày kết hôn không được ở tương lai");

        RuleFor(x => x.ChongId)
            .NotEqual(x => x.VoId).WithMessage("ChongId và VoId không được trùng nhau");
    }
}
