namespace GiaPha_Domain.Entities;

public class TaiKhoan_Ho
{
    // Composite Key
    public Guid TaiKhoanId { get; set; }
    public Guid HoId { get; set; }
    
    // Role trong họ này
    public RoleCuaHo RoleInHo { get; set; } = RoleCuaHo.ThanhVien;
    
    // Metadata
    public DateTime NgayThamGia { get; set; } = DateTime.UtcNow;
 
    // Navigation
    public TaiKhoanNguoiDung TaiKhoan { get; set; } = null!;
    public Ho Ho { get; set; } = null!;
}