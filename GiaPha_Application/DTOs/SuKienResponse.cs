using System;

namespace GiaPha_Application.DTOs
{
    public class SuKienResponse
    {
        public Guid Id { get; set; }
        public Guid ThanhVienId { get; set; }
        public string LoaiSuKien { get; set; } = null!;
        public DateTime NgayXayRa { get; set; }
        public string? DiaDiem { get; set; }
        public string? MoTa { get; set; }
    }
}