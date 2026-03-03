
using GiaPha_Domain.Common;
using static GiaPha_Domain.Events.UserEvents;

namespace GiaPha_Application.Events
{
    public class UserRegisteredEvent : IDomainEventWrapper<UserRegistered>
    {
        public UserRegistered DomainEvent { get; }
        IDomainEvent IDomainEventWrapper.DomainEvent => DomainEvent;
        
        public Guid id => DomainEvent.id;
        public string Email => DomainEvent.Email;
        public string TenDangNhap => DomainEvent.TenDangNhap;
        public string ActivationCode => DomainEvent.ActivationCode;
        public DateTime OccurredOn => DomainEvent.OccurredOn;

        public UserRegisteredEvent(UserRegistered domainEvent)
        {
            DomainEvent = domainEvent;
        }
    }
}