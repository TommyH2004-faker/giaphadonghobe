using GiaPha_Domain.Common;

namespace GiaPha_Domain.Events;
public static class ThanhVienEvent
{
    public record ThanhVienCreated : DomainEventBase
    {
        public Guid Id { get; init; }
        public string HoTen { get; init; }
        public string Email { get; init; } = string.Empty;
        public DateTime CreatedAt { get; init; }
        
        public ThanhVienCreated(Guid id, string hoTen, string email, DateTime createdAt)
        {
            Id = id;
            HoTen = hoTen;
            Email = email;
            CreatedAt = createdAt;

        }
    }
}