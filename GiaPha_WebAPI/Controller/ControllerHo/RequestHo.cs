namespace GiaPha_WebAPI.Controller.ControllerHo;

public class RequestHo
{
    public class CreateHoRequest
    {
        public Guid UserId { get; set; }
        public string TenHo { get; set; } = null!;
        public string? MoTa { get; set; }
        public string? QueQuan { get; set; }
        
        // Thông tin Thủy Tổ
        public string HoTenThuyTo { get; set; } = null!;
        public bool GioiTinhThuyTo { get; set; }
        public DateTime NgaySinhThuyTo { get; set; }
        public string? NoiSinhThuyTo { get; set; }
        public string? TieuSuThuyTo { get; set; }
    }
    
    public class UpdateHoRequest
    {
        public string TenHo { get; set; } = null!;
        public string? MoTa { get; set; }
        public Guid? ThuyToId { get; set; }
    }

    public class RequestJoinHoRequest
    {
        public Guid UserId { get; set; }
        public Guid HoId { get; set; }
        public string LyDoXinVao { get; set; } = null!;
    }

    public class RejectJoinRequestRequest
    {
        public string? GhiChu { get; set; }
    }
}