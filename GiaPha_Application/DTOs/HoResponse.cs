namespace GiaPha_Application.DTOs;
public class HoResponse
{
    public Guid Id { get; set; }
    public string? TenHo { get; set; }
    public string? MoTa { get; set; }
    public string? HinhAnh { get; set; }
    public string? QueQuan { get; set; }
    public Guid? ThuyToId { get; set; }
    public int? SoThanhVien { get; set; }
}