using FluentValidation;
namespace GiaPha_Application.Features.HoName.Command.AssignThuyTo;
public class AssignThuyToValidation : AbstractValidator<AssignThuyToCommand>
{
    public AssignThuyToValidation()
    {
        RuleFor(x => x.HoId)
            .NotEmpty().WithMessage("Họ ID không được để trống");

        RuleFor(x => x.ThuyToId)
            .NotEmpty().WithMessage("Thủy Tổ ID không được để trống");
    }
}