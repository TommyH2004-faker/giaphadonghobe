namespace GiaPha_Application.DTOs;

public class QuanHeChaConResponse
{
    public Guid Id { get; set; }
    public Guid ChaMeId { get; set; }
    public Guid ConId { get; set; }
    public int LoaiQuanHe { get; set; } // 0: Cha, 1: Mẹ
}