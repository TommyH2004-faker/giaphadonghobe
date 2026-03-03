using GiaPha_Domain.Common;
using static GiaPha_Domain.Events.HoEvent;

namespace GiaPha_Domain.Entities;

public class Ho : IHasDomainEvents
{
    // ====== Keys ======
    public Guid Id { get; private set; }

    // ====== Properties ======
    public string TenHo { get; private set; } = null!;
    public string? MoTa { get; private set; }
    public string? HinhAnh { get; private set; }
    public DateTime NgayTao { get; private set; } = DateTime.UtcNow;
    public string? QueQuan { get; private set; }

    // ====== FK Thủy Tổ ======
    public Guid? ThuyToId { get; set; }
    public ThanhVien? ThuyTo { get; set; }

    // ====== Navigation ======
    public ICollection<ThanhVien> ThanhViens { get; set; } = new List<ThanhVien>();
    
    // Navigation - Many to Many với TaiKhoanNguoiDung qua TaiKhoan_Ho
    public ICollection<TaiKhoan_Ho> TaiKhoan_Hos { get; set; } = new List<TaiKhoan_Ho>();

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    private List<IDomainEvent> _domainEvents = new List<IDomainEvent>();

    // ====== Constructor ======
    private Ho() { }

    // ====== Factory ======
    public static Ho Create(string tenHo, string? moTa, string? queQuan)
    {
        if (string.IsNullOrWhiteSpace(tenHo))
            throw new ArgumentException("TenHo cannot be empty");

        return new Ho
        {
            Id = Guid.NewGuid(),
            TenHo = tenHo,
            MoTa = moTa,
            QueQuan = queQuan
        };
    }


    public void SetThuyTo(Guid thanhVienId)
    {
        ThuyToId = thanhVienId;
    }

    public void Update(Guid? idThuyTo, string tenHo, string moTa , string queQuan )
    {
        if(idThuyTo == Guid.Empty)
            throw new ArgumentException("ThuyToId cannot be empty");
        if (string.IsNullOrWhiteSpace(tenHo))
            throw new ArgumentException("TenHo cannot be empty");
        if(string.IsNullOrWhiteSpace(queQuan))
            throw new ArgumentException("QueQuan cannot be empty");
        if(string.IsNullOrWhiteSpace(moTa))
            throw new ArgumentException("MoTa cannot be empty");
        TenHo = tenHo;
        MoTa = moTa;
        QueQuan = queQuan;
        ThuyToId = idThuyTo;
        
        // ⚡ Raise event tổng quát cho việc update họ
        AddDomainEvent(new HoUpdatedEvent(
            this.Id,
            tenHo,
            moTa,
            queQuan,
            idThuyTo,
            DateTime.UtcNow
        ));
    }
    // Assign thủy tổ và raise domain event
    public void AssignThuyTo(Guid thanhVienId)
    {
        if (thanhVienId == Guid.Empty)
            throw new ArgumentException("ThanhVienId cannot be empty");

        ThuyToId = thanhVienId;
        
        // ⚡ Raise Domain Event
        AddDomainEvent(new AssignedThuyToHoEvent(
            this.Id,
            thanhVienId,
            DateTime.UtcNow
        ));
    }

    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}