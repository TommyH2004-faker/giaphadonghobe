namespace GiaPha_Domain.Entities;

public class TepTin
{
    public Guid Id { get; private set; }
    public string DuongDan { get; private set; } = null!;
    public string? MoTa { get; private set; }

    public Guid ThanhVienId { get; set; }
    public ThanhVien ThanhVien { get; set; } = null!;
}
