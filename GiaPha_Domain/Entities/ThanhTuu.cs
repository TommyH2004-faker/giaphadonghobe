namespace GiaPha_Domain.Entities;

public class ThanhTuu
{
    public Guid Id { get; private set; }

    public Guid ThanhVienId  { get; set; } 
    public ThanhVien ThanhVien { get; set; } = null!;

    public string TenThanhTuu  { get; set; } = null!;
    public int? NamDatDuoc { get; private set; }
    public string? MoTa { get; private set; }

    private ThanhTuu() { }

    public static ThanhTuu Create(
        Guid thanhVienId,
        string tenThanhTuu,
        int? namDatDuoc = null,
        string? moTa = null)
    {
        if (string.IsNullOrWhiteSpace(tenThanhTuu))
            throw new ArgumentException("Tên thành tựu không được để trống");

        return new ThanhTuu
        {
            Id = Guid.NewGuid(),
            ThanhVienId = thanhVienId,
            TenThanhTuu = tenThanhTuu,
            NamDatDuoc = namDatDuoc,
            MoTa = moTa
        };
    }

    public void Update(string tenThanhTuu, int? namDatDuoc, string? moTa)
    {
        if (string.IsNullOrWhiteSpace(tenThanhTuu))
            throw new ArgumentException("Tên thành tựu không được để trống");

        TenThanhTuu = tenThanhTuu;
        NamDatDuoc = namDatDuoc;
        MoTa = moTa;
    }
}
