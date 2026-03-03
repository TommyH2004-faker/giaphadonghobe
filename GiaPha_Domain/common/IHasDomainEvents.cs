namespace GiaPha_Domain.Common
{
    /// <summary>
    /// // Interface cho Entity có khả năng phát sinh events
    /// </summary>
    public interface IHasDomainEvents
    {
        IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
        void AddDomainEvent(IDomainEvent domainEvent);
        void RemoveDomainEvent(IDomainEvent domainEvent);
        void ClearDomainEvents();
    }
}
