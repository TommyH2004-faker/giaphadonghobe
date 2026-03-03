namespace GiaPha_Domain.Entities;

public class AuditLog
{
    public Guid Id{ get; private set; } = Guid.NewGuid();
    public string EntityName { get; private set; } = null!;
    public Guid EntityId { get; private set; }
    public string Action { get; private set; } = null!; // Create, Update, Delete
    public string? ChangedBy { get; private set; }
    public DateTime ChangedAt { get; private set; }
    public string? OldValues { get; private set; } // JSON mô tả giá trị cũ
    public string? NewValues { get; private set; } // JSON mô tả giá trị mới
    private AuditLog() { }
     public static AuditLog Create(
            string action,
            string entityName,
            Guid entityId,
            string? oldValues,
            string? newValues,
            string? performedBy = null)
        {
            return new AuditLog
            {
                Action = action,
                EntityName = entityName,
                EntityId = entityId,
                OldValues = oldValues,
                NewValues = newValues,
                ChangedAt = DateTime.UtcNow,
                ChangedBy = performedBy
            };
        }
}
