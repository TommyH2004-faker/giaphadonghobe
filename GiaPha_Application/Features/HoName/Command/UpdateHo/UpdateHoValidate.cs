using FluentValidation;

namespace GiaPha_Application.Features.HoName.Command.UpdateHo;
public class UpdateHoValidate : AbstractValidator<UpdateHoCommand>
{
    public UpdateHoValidate()
    {
        RuleFor(x => x.TenHo)
            .NotEmpty().WithMessage("Tên Họ không được để trống")
            .MaximumLength(100).WithMessage("Tên Họ không được vượt quá 100 ký tự");

        RuleFor(x => x.MoTa)
            .MaximumLength(500).WithMessage("Mô tả không được vượt quá 500 ký tự");
        RuleFor(x => x.queQuan)
            .MaximumLength(200).WithMessage("Quê quán không được vượt quá 200 ký tự");
        RuleFor(x => x.hinhAnh)
            .MaximumLength(500).WithMessage("Hình ảnh không được vượt quá 500 ký tự");

        
    }
}
