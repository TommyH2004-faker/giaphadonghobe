

using GiaPha_Domain.Common;
using static GiaPha_Domain.Events.UserEvents;

namespace GiaPha_Application.Events
{
    public class UserActivatedEvent : IDomainEventWrapper<UserActivated>
    {
        public UserActivated DomainEvent { get; }
        IDomainEvent IDomainEventWrapper.DomainEvent => DomainEvent;
        
        public Guid id => DomainEvent.id;
        public string Email => DomainEvent.Email;
        public DateTime ActivatedAt => DomainEvent.ActivatedAt;
        public DateTime OccurredOn => DomainEvent.OccurredOn;

        public UserActivatedEvent(UserActivated domainEvent)
        {
            DomainEvent = domainEvent;
        }
    }
}