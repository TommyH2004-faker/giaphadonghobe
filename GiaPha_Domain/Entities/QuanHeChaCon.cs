namespace GiaPha_Domain.Entities;

public class QuanHeChaCon
{
    public Guid Id { get; private set; }

    public Guid ChaMeId { get; set; }
    public ThanhVien ChaMe { get; set; } = null!;

    public Guid ConId { get; set; }
    public ThanhVien Con { get; set; } = null!;

    public int LoaiQuanHe { get; private set; }

    private QuanHeChaCon() { }

    public static QuanHeChaCon Create(Guid chaMeId, Guid conId, int loai)
    {
        return new QuanHeChaCon
        {
            Id = Guid.NewGuid(),
            ChaMeId = chaMeId,
            ConId = conId,
            LoaiQuanHe = loai
        };
    }
}
