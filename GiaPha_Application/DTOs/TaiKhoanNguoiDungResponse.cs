namespace GiaPha_Application.DTOs;
public class TaiKhoanNguoiDungResponse
{
    public Guid Id { get; set; }
    public string TenDangNhap { get; set; } = null!;
    public string? Gmail { get; set; }
    public bool Enabled { get; set; }
    public string? Avatar { get; set; }
    public bool GioiTinh { get; set; }
    public string? SoDienThoai { get; set; }
    public List<HoWithRoleResponse> AvailableHos { get; set; } = new List<HoWithRoleResponse>();
}