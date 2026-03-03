using FluentValidation;

namespace GiaPha_Application.Features.SuKien.Command.Create;
public class CreateSuKienValidation : AbstractValidator<CreateSuKienCommand>
{
    public CreateSuKienValidation()
    {
        RuleFor(x => x.ThanhVienId).NotEmpty().WithMessage("ThanhVienId không được để trống.");
        RuleFor(x => x.LoaiSuKien).NotEmpty().WithMessage("LoaiSuKien không được để trống.");
        RuleFor(x => x.NgayXayRa).NotEmpty().WithMessage("NgayXayRa không được để trống.");
        RuleFor(x => x.DiaDiem).MaximumLength(200).WithMessage("DiaDiem không được vượt quá 200 ký tự.");
        RuleFor(x => x.MoTa).MaximumLength(500).WithMessage("MoTa không được vượt quá 500 ký tự.");
    }
}