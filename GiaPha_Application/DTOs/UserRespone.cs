namespace GiaPha_Application.DTOs;
public class UserResponse
{
    public Guid Id { get; set; }
    public string? TenDangNhap { get; set; }
    public string? Email { get; set; }
    public string Role { get; set; } = null!;
    public bool Enabled { get; set; }
    public string MatKhauMaHoa { get; set; } = null!;
    public string? SoDienThoai { get; set; }
}