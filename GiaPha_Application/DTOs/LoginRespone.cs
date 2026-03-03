

namespace GiaPha_Application.DTOs;
public class LoginRespone
{
    public string? TenDangNhap { get; set; }
    public string? Email { get; set; }
    public string? Role { get; set; }
    public string? Token { get; set; }
    
    // Danh sách các Ho mà user thuộc về (với role)
    public List<HoWithRoleResponse> AvailableHos { get; set; } = new();
    
    // Ho hiện tại đang chọn (nếu có)
    public Guid? SelectedHoId { get; set; }
    
    // Role trong họ hiện tại: 0 = Trưởng họ (full quyền), 1 = Thành viên (chỉ xem)
    public int? RoleInCurrentHo { get; set; }
}