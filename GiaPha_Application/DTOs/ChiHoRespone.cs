namespace GiaPha_Application.DTOs;
public class ChiHoResponse
{
    public Guid Id { get; set; }
    public Guid IdHo { get; set; }
    public string? TenChiHo { get; set; }
    public string? MoTa { get; set; }
    public Guid? TruongChiId { get; set; }

}