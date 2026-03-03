

using GiaPha_Domain.Common;
using static GiaPha_Domain.Events.UserEvents;

namespace GiaPha_Application.Events
{
    public class UserPasswordChangedEvent : IDomainEventWrapper<UserPasswordChanged>
    {
        public UserPasswordChanged DomainEvent { get; }
        IDomainEvent IDomainEventWrapper.DomainEvent => DomainEvent;
        
        public Guid id => DomainEvent.id;
        public string Email => DomainEvent.Email;
        public DateTime ChangedAt => DomainEvent.ChangedAt;
        public DateTime OccurredOn => DomainEvent.OccurredOn;

        public UserPasswordChangedEvent(UserPasswordChanged domainEvent)
        {
            DomainEvent = domainEvent;
        }
    }
}