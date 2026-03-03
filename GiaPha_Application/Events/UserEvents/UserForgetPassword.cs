using GiaPha_Domain.Common;
using static GiaPha_Domain.Events.UserEvents;

namespace GiaPha_Application.Events.UserEvents
{
    public class UserForgotPasswordEvent : IDomainEventWrapper<UserForgotPassword>
    {
        public UserForgotPassword DomainEvent { get; }
        IDomainEvent IDomainEventWrapper.DomainEvent => DomainEvent;
        public string plainPassword => DomainEvent.plainPassword;
        public Guid id => DomainEvent.Id;
        public string Email => DomainEvent.Email;
        public DateTime OccurredOn => DomainEvent.OccurredOn;


        public UserForgotPasswordEvent(UserForgotPassword domainEvent)
        {
            DomainEvent = domainEvent;
        }
    }
}