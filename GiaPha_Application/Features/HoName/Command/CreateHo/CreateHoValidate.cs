using FluentValidation;

namespace GiaPha_Application.Features.HoName.Command.CreateHo;

public class CreateHoValidate : AbstractValidator<CreateHoCommand>
{
    public CreateHoValidate()
    {
        // Validation Họ
        RuleFor(x => x.TenHo)
            .NotEmpty().WithMessage("Tên họ không được để trống")
            .MaximumLength(100).WithMessage("Tên họ không được vượt quá 100 ký tự");

        RuleFor(x => x.MoTa)
            .MaximumLength(500).WithMessage("Mô tả không được vượt quá 500 ký tự");

        RuleFor(x => x.QueQuan)
            .MaximumLength(200).WithMessage("Quê quán không được vượt quá 200 ký tự");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId là bắt buộc");

        // Validation Thủy Tổ
        RuleFor(x => x.HoTenThuyTo)
            .NotEmpty().WithMessage("Họ tên Thủy Tổ không được để trống")
            .MaximumLength(100).WithMessage("Họ tên không được vượt quá 100 ký tự");

        RuleFor(x => x.NgaySinhThuyTo)
            .NotEmpty().WithMessage("Ngày sinh Thủy Tổ không được để trống")
            .LessThan(DateTime.Now).WithMessage("Ngày sinh phải nhỏ hơn ngày hiện tại")
            .GreaterThan(new DateTime(1500, 1, 1)).WithMessage("Ngày sinh không hợp lệ");
    }
}