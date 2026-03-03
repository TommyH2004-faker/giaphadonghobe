using GiaPha_Application.Repository;
using GiaPha_Domain.Entities;
using GiaPha_Infrastructure.Db;
using Microsoft.EntityFrameworkCore;

namespace GiaPha_Infrastructure.Repository;

public class AuditReopository : IAuditLogRepository
{
    private readonly DbGiaPha dbGiaPha;
    public AuditReopository(DbGiaPha dbGiaPha)
    {
        this.dbGiaPha = dbGiaPha;
    }   
    public async Task AddAsync(AuditLog auditLog)
    {
      dbGiaPha.AuditLogs.Add(auditLog);
    }

    public async Task<IReadOnlyList<AuditLog>> GetAllAsync(int page = 1, int pageSize = 50)
    {
        dbGiaPha.AuditLogs.Skip((page - 1) * pageSize).Take(pageSize);
        return await dbGiaPha.AuditLogs.ToListAsync();
    }

    public async Task<IReadOnlyList<AuditLog>> GetByEntityAsync(string entityName, Guid entityId)
    {
        return await dbGiaPha.AuditLogs
            .Where(a => a.EntityName == entityName && a.EntityId == entityId)
            .ToListAsync();
    }
}