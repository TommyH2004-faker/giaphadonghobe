namespace GiaPha_Domain.Entities;

public class SuKien
{
    public Guid Id { get; private set; }
    public Guid ThanhVienId { get; set; }
    public ThanhVien? ThanhVien { get; private set; } 

    public string LoaiSuKien { get; private set; } = null!;
    public DateTime NgayXayRa { get; private set; }
    public string? DiaDiem { get; private set; }
    public string? MoTa { get; private set; }

    private SuKien() { }
    public static SuKien Create(Guid thanhVienId, string loaiSuKien, DateTime ngayXayRa, string? diaDiem, string? moTa)
    {
        if (string.IsNullOrWhiteSpace(loaiSuKien))
        {
            throw new ArgumentException("Loại sự kiện không được để trống", nameof(loaiSuKien));
        }
        return new SuKien
        {
            Id = Guid.NewGuid(),
            ThanhVienId = thanhVienId,
            LoaiSuKien = loaiSuKien,
            NgayXayRa = ngayXayRa,
            DiaDiem = diaDiem,
            MoTa = moTa
        };
    }
    public void Update(Guid thanhVienId, string loaiSuKien, DateTime ngayXayRa, string? diaDiem, string? moTa)
    {
        if (string.IsNullOrWhiteSpace(loaiSuKien))
        {
            throw new ArgumentException("Loại sự kiện không được để trống", nameof(loaiSuKien));
        }
        ThanhVienId = thanhVienId;
        LoaiSuKien = loaiSuKien;
        NgayXayRa = ngayXayRa;
        DiaDiem = diaDiem;
        MoTa = moTa;
    }
}
