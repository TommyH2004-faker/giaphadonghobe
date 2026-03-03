
using FluentValidation;

namespace GiaPha_Application.Features.ThanhVien.Command.Create;
public class CreateThanhVienValidation: AbstractValidator<CreateThanhVienCommand>
{
    public CreateThanhVienValidation()
    {

        RuleFor(x => x.HoTen)
            .NotEmpty().WithMessage("Họ tên không được để trống")
            .MaximumLength(200).WithMessage("Họ tên không được vượt quá 200 ký tự");
        RuleFor(x => x.NoiSinh)
            .MaximumLength(200).WithMessage("Nơi sinh không được vượt quá 200 ký tự");
        RuleFor(x => x.TieuSu)
            .MaximumLength(1000).WithMessage("Tiểu sử không được vượt quá 1000 ký tự");
        RuleFor(x => x.TrangThai)
            .NotEmpty().WithMessage("Trạng thái không được để trống");
        // HoId is optional - can be null for spouse from different family (avoid incest)
        // ChiHoId is optional - can be null when creating new member without branch assignment
        
    }
}