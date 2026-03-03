using GiaPha_Domain.Common;
using static GiaPha_Domain.Events.HoEvent;

namespace GiaPha_Application.Events.HoEvents;

public class HoUpdatedEventWrapper : IDomainEventWrapper<HoUpdatedEvent>
{
    public HoUpdatedEvent DomainEvent { get; }
    IDomainEvent IDomainEventWrapper.DomainEvent => DomainEvent;

    public Guid HoId => DomainEvent.HoId;
    public string TenHo => DomainEvent.TenHo;
    public string? MoTa => DomainEvent.MoTa;
    public string? QueQuan => DomainEvent.QueQuan;
    public Guid? ThuyToId => DomainEvent.ThuyToId;
    public DateTime UpdatedAt => DomainEvent.UpdatedAt;
    public DateTime OccurredOn => DomainEvent.OccurredOn;

    public HoUpdatedEventWrapper(HoUpdatedEvent domainEvent)
    {
        DomainEvent = domainEvent;
    }
}
