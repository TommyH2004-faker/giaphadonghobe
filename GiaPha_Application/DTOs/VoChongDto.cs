namespace GiaPha_Application.DTOs;

public class VoChongDto
{
    public Guid HonNhanId { get; set; }
    public Guid VoChongId { get; set; }
    public string HoTen { get; set; } = string.Empty;
    public DateTime? NgaySinh { get; set; }
    public DateTime? NgayMat { get; set; }
    public string? Avatar { get; set; }
    public string? TieuSu { get; set; } 
    public DateTime NgayKetHon { get; set; } 
    public DateTime? NgayLyHon { get; set; }
    public bool TrangThaiHonNhan { get; set; }
    public bool IsDeleted { get; set; } 
    
    // Con của cuộc hôn nhân này
    public List<Guid> ConIds { get; set; } = new();
}