using GiaPha_Domain.Entities;

namespace GiaPha_Application.Repository
{
    public interface IAuditLogRepository
    {
        Task AddAsync(AuditLog auditLog);
        Task<IReadOnlyList<AuditLog>> GetByEntityAsync(string entityName, Guid entityId);
        Task<IReadOnlyList<AuditLog>> GetAllAsync(int page = 1, int pageSize = 50);
    }
}