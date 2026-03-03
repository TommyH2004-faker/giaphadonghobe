using GiaPha_Application.Events.ThanhVienEvents;
using GiaPha_Application.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Service;

namespace GiaPha_Application.Features.ThanhVien.Command.EventHandler.Create;

public class CreateThanhVienNotificationHandler : INotificationHandler<CreateThanhVienEvent>
{
    private readonly IEmailService  _emailService;
    private readonly ILogger<CreateThanhVienNotificationHandler> _logger;
    public CreateThanhVienNotificationHandler(IEmailService emailService, ILogger<CreateThanhVienNotificationHandler> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }
    public async Task Handle(CreateThanhVienEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("📧 [THANHVIEN] Gửi email thông báo thành viên mới ID {Id}", notification.Id);
        var subject = "Thông báo thành viên mới";
        var body = $"Thành viên mới đã được tạo: {notification.HoTen} với ID: {notification.Id} vào lúc {notification.CreatedAt}.";
       await _emailService.SendEmailAsync(notification.Email, subject, body);
       _logger.LogInformation("✅ [THANHVIEN] Email thông báo thành viên mới đã được gửi đến {Email}", notification.Email);
        
    }
}