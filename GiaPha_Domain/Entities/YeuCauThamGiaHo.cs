namespace GiaPha_Domain.Entities;

/// <summary>
/// Trạng thái yêu cầu tham gia họ
/// </summary>
public enum TrangThaiYeuCau
{
    DangCho = 0,   // Đang chờ duyệt
    DaDuyet = 1,   // Đã duyệt
    TuChoi = 2     // Từ chối
}

/// <summary>
/// Yêu cầu tham gia vào một dòng họ
/// </summary>
public class YeuCauThamGiaHo
{
    public Guid Id { get; private set; }

    // Người gửi yêu cầu
    public Guid UserId { get; private set; }
    public TaiKhoanNguoiDung? User { get; set; }

    // Họ muốn tham gia
    public Guid HoId { get; private set; }
    public Ho? Ho { get; set; }

    // Nội dung
    public string LyDoXinVao { get; private set; } = null!;
    public TrangThaiYeuCau TrangThai { get; private set; } = TrangThaiYeuCau.DangCho;

    // Metadata
    public DateTime NgayTao { get; private set; }
    public DateTime? NgayXuLy { get; private set; }
    public Guid? NguoiXuLyId { get; private set; }
    public TaiKhoanNguoiDung? NguoiXuLy { get; set; }
    public string? GhiChuTuChoi { get; private set; }

    private YeuCauThamGiaHo() { }

    public static YeuCauThamGiaHo Create(Guid userId, Guid hoId, string lyDoXinVao)
    {
        if (string.IsNullOrWhiteSpace(lyDoXinVao))
            throw new ArgumentException("Lý do xin vào không được để trống");

        return new YeuCauThamGiaHo
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            HoId = hoId,
            LyDoXinVao = lyDoXinVao,
            TrangThai = TrangThaiYeuCau.DangCho,
            NgayTao = DateTime.UtcNow
        };
    }

    public void Duyet(Guid nguoiXuLyId)
    {
        TrangThai = TrangThaiYeuCau.DaDuyet;
        NgayXuLy = DateTime.UtcNow;
        NguoiXuLyId = nguoiXuLyId;
    }

    public void TuChoi(Guid nguoiXuLyId, string? ghiChu)
    {
        TrangThai = TrangThaiYeuCau.TuChoi;
        NgayXuLy = DateTime.UtcNow;
        NguoiXuLyId = nguoiXuLyId;
        GhiChuTuChoi = ghiChu;
    }
}
