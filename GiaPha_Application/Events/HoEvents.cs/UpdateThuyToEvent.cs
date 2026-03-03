using GiaPha_Domain.Common;
using GiaPha_Domain.Entities;
using static GiaPha_Domain.Events.HoEvent;

namespace GiaPha_Application.Events.HoEvents;

public class UpdateThuyToEvent : IDomainEventWrapper<HoUpdatedThuyToEvent>
{
    public HoUpdatedThuyToEvent DomainEvent { get; }
    IDomainEvent IDomainEventWrapper.DomainEvent => DomainEvent;
    public Guid ThuyToId => DomainEvent.ThuyToId;
    public string TenHo => DomainEvent.TenHo;
    public string? MoTa => DomainEvent.MoTa;
    public DateTime UpdatedAt => DomainEvent.UpdatedAt;
    public UpdateThuyToEvent(HoUpdatedThuyToEvent domainEvent)
    {
        DomainEvent = domainEvent;
    }
    
}