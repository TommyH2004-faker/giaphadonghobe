using GiaPha_Domain.Common;
using static GiaPha_Domain.Events.HoEvent;

namespace GiaPha_Application.Events.HoEvents;
public class AssignThuyToEvent : IDomainEventWrapper<AssignedThuyToHoEvent>
{
    public AssignedThuyToHoEvent DomainEvent { get; }

    IDomainEvent IDomainEventWrapper.DomainEvent => DomainEvent;
    public Guid HoId => DomainEvent.HoId;
    public Guid ThuyToId => DomainEvent.ThuyToId;
    public DateTime AssignedAt => DomainEvent.AssignedAt;

    public AssignThuyToEvent(AssignedThuyToHoEvent domainEvent)
    {
        DomainEvent = domainEvent;
    }
    
}