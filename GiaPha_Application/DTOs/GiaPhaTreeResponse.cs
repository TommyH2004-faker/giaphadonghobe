namespace GiaPha_Application.DTOs;

public class GiaPhaTreeResponse
{
    public string TenHo { get; set; } = string.Empty;
    public Guid HoId { get; set; }
    public GiaPhaNodeDto ThuyTo { get; set; } = null!;
    
    // Metadata
    public int TongSoThanhVien { get; set; }
    public int SoCapDo { get; set; }
    public DateTime NgayTao { get; set; } = DateTime.UtcNow;
}