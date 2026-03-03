using GiaPha_Domain.Common;

namespace GiaPha_Domain.Events;
public class HoEvent
{
    public record AssignedThuyToHoEvent : DomainEventBase
    {
        public Guid HoId { get; init; }
        public Guid ThuyToId { get; init; }
        // time assigned
        public DateTime AssignedAt { get; init; }

        public AssignedThuyToHoEvent(Guid hoId, Guid thuyToId, DateTime assignedAt)
        {
            HoId = hoId;
            ThuyToId = thuyToId;  
            AssignedAt = assignedAt;    
            }
    }
    public record HoUpdatedThuyToEvent : DomainEventBase
    {
        public Guid ThuyToId { get; init; }
        public string TenHo { get; init; }
        public string? MoTa { get; init; }
        public DateTime UpdatedAt { get; init; }
        public HoUpdatedThuyToEvent(Guid thuyToId, string tenHo, string? moTa, DateTime updatedAt)
        {
            ThuyToId = thuyToId;
            this.TenHo = tenHo;
            this.MoTa = moTa;
            UpdatedAt = updatedAt;
        }
    }
    
    public record HoUpdatedEvent : DomainEventBase
    {
        public Guid HoId { get; init; }
        public string TenHo { get; init; }
        public string? MoTa { get; init; }
        public string? QueQuan { get; init; }
        public Guid? ThuyToId { get; init; }
        public DateTime UpdatedAt { get; init; }
        
        public HoUpdatedEvent(Guid hoId, string tenHo, string? moTa, string? queQuan, Guid? thuyToId, DateTime updatedAt)
        {
            HoId = hoId;
            TenHo = tenHo;
            MoTa = moTa;
            QueQuan = queQuan;
            ThuyToId = thuyToId;
            UpdatedAt = updatedAt;
        }
    }
}