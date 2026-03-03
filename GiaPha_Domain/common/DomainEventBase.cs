namespace GiaPha_Domain.Common
{
    /// <summary>
    /// // Base class cho tất cả events - tự động ghi thời gian
    /// Đánh dấu sự kiện trong miền (Domain Event)
    /// </summary>
    public abstract record DomainEventBase : IDomainEvent
    {
        public DateTime OccurredOn { get; init; }

        protected DomainEventBase()
        {
            OccurredOn = DateTime.UtcNow;
        }
    }
}
