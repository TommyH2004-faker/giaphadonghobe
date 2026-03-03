namespace GiaPha_Application.DTOs;

public class GiaPhaNodeDto
{
    public Guid Id { get; set; }
    public string HoTen { get; set; } = string.Empty;
    public bool GioiTinh { get; set; } // false: Nam, true: Nữ
    public DateTime? NgaySinh { get; set; }
    public DateTime? NgayMat { get; set; }
    public int Level { get; set; } // Cấp độ trong cây (thủy tổ = 0)
    public string? Avatar { get; set; }
    public string? TieuSu { get; set; } // ✅ Tiểu sử
    
    // Thông tin họ
    public Guid HoId { get; set; }
    public bool IsDeleted { get; set; } // ✅ Đánh dấu người đã bị xóa
    
    // Thông tin hôn nhân
    public List<VoChongDto> DanhSachVoChong { get; set; } = new();
    
    // Con cái
    public List<GiaPhaNodeDto> Con { get; set; } = new();
    
    // Metadata
    public bool HasChildren { get; set; }
    public int TongSoCon { get; set; }
}