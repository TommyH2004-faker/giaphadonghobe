namespace GiaPha_Application.DTOs;
public class HonNhanResponse
{
    public Guid Id { get; set; }
    public Guid ChongId { get; set; }
    public Guid VoId { get; set; }
    public DateTime NgayKetHon { get; set; }
    public bool TrangThai { get; set; }
    public DateTime? NgayLyHon { get; set; }
}