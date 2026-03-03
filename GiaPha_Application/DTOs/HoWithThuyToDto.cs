namespace GiaPha_Application.DTOs;

/// <summary>
/// DTO hiển thị Họ kèm thông tin Thủy Tổ (dùng cho dropdown chọn họ)
/// </summary>
public class HoWithThuyToDto
{
    public Guid HoId { get; set; }
    public string TenHo { get; set; } = null!;
    public string? QueQuan { get; set; }
    
    // Thông tin Thủy Tổ
    public Guid? ThuyToId { get; set; }
    public string? TenThuyTo { get; set; }
    public DateTime? NgaySinhThuyTo { get; set; }
    
    // Text hiển thị: "Họ Nguyễn - Thủy Tổ: Nguyễn Văn A (1850)"
    public string DisplayText => !string.IsNullOrEmpty(TenThuyTo) 
        ? $"{TenHo} - Thủy Tổ: {TenThuyTo} ({NgaySinhThuyTo?.Year})"
        : TenHo;
}
