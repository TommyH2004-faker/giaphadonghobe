namespace GiaPha_Application.DTOs;

public class HoWithRoleResponse
{
    public Guid Id { get; set; }
    public string? TenHo { get; set; }
    public string? MoTa { get; set; }
    public string? HinhAnh { get; set; }
    public string? QueQuan { get; set; }
    public Guid? ThuyToId { get; set; }
    
    /// <summary>
    /// Role của user trong họ này: 0 = Trưởng họ, 1 = Thành viên
    /// </summary>
    public int RoleInHo { get; set; }
}
