namespace GiaPha_Application.DTOs;
public class ThanhVienResponse
{
    public Guid Id { get; set; }
    public string? Avatar { get; set; }
    public string HoTen { get; set; } = null!;
    public bool GioiTinh { get; set; }
    public DateTime NgaySinh { get; set; }
    public string? NoiSinh { get; set; }
    public DateTime? NgayMat { get; set;}
    public string? NoiMat { get; set; }
    public int DoiThu { get;set; }
    public string? TieuSu { get; set; }
    public bool TrangThai { get; set; }
    public Guid HoId { get; set; }
}