using GiaPha_Domain.Common;
using MediatR;


namespace GiaPha_Application.Events
{
    /// <summary>
    ///Interface đánh dấu wrapper (kế thừa INotification của MediatR)
    /// </summary>
    public interface IDomainEventWrapper : INotification
    {
        IDomainEvent DomainEvent { get; }
    }

    /// <summary>
    /// Generic wrapper cho type-safe
    public interface IDomainEventWrapper<TDomainEvent> : IDomainEventWrapper
        where TDomainEvent : IDomainEvent
    {
        new TDomainEvent DomainEvent { get; }
    }
}
