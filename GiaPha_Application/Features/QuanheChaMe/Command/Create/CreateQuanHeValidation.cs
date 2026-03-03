using FluentValidation;

namespace GiaPha_Application.Features.QuanheChaMe.Command.Create;
public class CreateQuanHeValidation:AbstractValidator<CreateQuanHeChaConCommand>
{
    public CreateQuanHeValidation()
    {
        RuleFor(x => x.ChaMeId)
            .NotEmpty().WithMessage("Cha/Mẹ Id không được để trống");

        RuleFor(x => x.ConId)
            .NotEmpty().WithMessage("Con Id không được để trống");

        RuleFor(x => x.LoaiQuanHe)
            .InclusiveBetween(0, 1).WithMessage("Loại quan hệ phải là 0 (Cha) hoặc 1 (Mẹ)");
    }
    
}