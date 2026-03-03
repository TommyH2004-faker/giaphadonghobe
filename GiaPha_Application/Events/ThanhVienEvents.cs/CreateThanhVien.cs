using GiaPha_Domain.Common;
using MediatR;
using static GiaPha_Domain.Events.ThanhVienEvent;

namespace GiaPha_Application.Events.ThanhVienEvents;

public class CreateThanhVienEvent : IDomainEventWrapper<ThanhVienCreated>
{
   

    public ThanhVienCreated DomainEvent { get; }

    IDomainEvent IDomainEventWrapper.DomainEvent => DomainEvent;
    public Guid Id => DomainEvent.Id;
    public string Email => DomainEvent.Email;
    public string HoTen => DomainEvent.HoTen;
    public DateTime CreatedAt => DomainEvent.CreatedAt;
    public DateTime OccurredOn => DomainEvent.OccurredOn;
    public CreateThanhVienEvent(ThanhVienCreated domainEvent)
    {
        DomainEvent = domainEvent;
    }
}