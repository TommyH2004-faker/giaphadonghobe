using System.Text.Json;
using GiaPha_Application.Events;
using GiaPha_Application.Repository;
using GiaPha_Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GiaPha_Application.Features.Auth.Command.EventHandlers.Register;

public class UserRegisterAuditLogHandler : INotificationHandler<UserRegisteredEvent>
{
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly ILogger<UserRegisterAuditLogHandler> _logger;

    public UserRegisterAuditLogHandler(
        IAuditLogRepository auditLogRepository,
        ILogger<UserRegisterAuditLogHandler> logger)
    {
        _auditLogRepository = auditLogRepository;
        _logger = logger;
    }

    public async Task Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("📝 [AUDIT] Recording CREATE for User #{UserId}", notification.id);

        var newValues = JsonSerializer.Serialize(new
        {
            notification.id,
            notification.TenDangNhap,
            notification.Email,
            Status = "Registered"
        });
        //    string action,
        //     string entityName,
        //     Guid entityId,
        //     string? oldValues,
        //     string? newValues,
        //     string? performedBy = null)
        var auditLog = AuditLog.Create(
            entityName: "User",
            entityId: notification.id,
            action: "CREATE",
            newValues: newValues,
            performedBy: notification.TenDangNhap,
            oldValues: null
        );

        await _auditLogRepository.AddAsync(auditLog);
    }
}