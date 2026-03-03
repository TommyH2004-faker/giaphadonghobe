namespace GiaPha_Domain.Entities;

public class Notification
{
    public Guid Id { get; private set; }
    public string NoiDung { get; private set; } = null!;

    // ===== Người nhận cụ thể =====
    public Guid? NguoiNhanId { get; set; }
    public ThanhVien? NguoiNhan { get; set; }

    // ===== Phạm vi =====
    public bool IsGlobal { get; private set; }


    public Guid? HoId { get; set; }
    public Ho? Ho { get; set; }

    // ===== Trạng thái =====
    public bool DaDoc { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Notification() { }

    public static Notification Create(
        string noiDung,
        bool isGlobal = false,
        Guid? nguoiNhanId = null,
        Guid? hoId = null)
    {
        if (string.IsNullOrWhiteSpace(noiDung))
            throw new ArgumentException("Nội dung không được rỗng");

        return new Notification
        {
            Id = Guid.NewGuid(),
            NoiDung = noiDung,
            IsGlobal = isGlobal,
            NguoiNhanId = nguoiNhanId,
            HoId = hoId,
            DaDoc = false,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void MarkAsRead()
    {
        DaDoc = true;
    }
}
