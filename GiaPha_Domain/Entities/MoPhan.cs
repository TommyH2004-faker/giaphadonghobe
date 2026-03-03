namespace GiaPha_Domain.Entities;

public class MoPhan
{
    public Guid Id { get; private set; }

    public string? MoTa { get; private set; }
    public string ViTri { get; private set; } = null!;
    public string KinhDo { get; private set; } = null!;
    public string ViDo { get; private set; } = null!;

    // ===== FK =====
    public Guid ThanhVienId { get; set; }

    // ===== Navigation =====
    public ThanhVien ThanhVien { get; set; } = null!;

    private MoPhan() { }

    public static MoPhan Create(
        Guid thanhVienId,
        string viTri,
        string kinhDo,
        string viDo,
        string? moTa = null)
    {
        return new MoPhan
        {
            Id = Guid.NewGuid(),
            ThanhVienId = thanhVienId,
            ViTri = viTri,
            KinhDo = kinhDo,
            ViDo = viDo,
            MoTa = moTa
        };
    }
}
