using GiaPha_Application.Service;
using GiaPha_Infrastructure.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Service;

namespace GiaPha_Infrastructure.Service;

/// <summary>
/// Implementation của ISuKienReminderService.
/// Thuộc Infrastructure Layer (Clean Architecture).
/// Chịu trách nhiệm truy vấn DB và gửi email qua IEmailService.
/// </summary>
public class SuKienReminderService : ISuKienReminderService
{
    private readonly DbGiaPha _dbContext;
    private readonly IEmailService _emailService;
    private readonly ILogger<SuKienReminderService> _logger;

    public SuKienReminderService(
        DbGiaPha dbContext,
        IEmailService emailService,
        ILogger<SuKienReminderService> logger)
    {
        _dbContext = dbContext;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task SendUpcomingEventRemindersAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("🔍 Checking for events happening in exactly 3 days...");

        // Lấy ngày mục tiêu: đúng 3 ngày kể từ hôm nay (chỉ so sánh ngày)
        var targetDate = DateTime.UtcNow.Date.AddDays(3);
        var nextDay = targetDate.AddDays(1);

        // Query sự kiện diễn ra đúng ngày thứ 3, include ThanhVien để lấy HoId
        var upcomingEvents = await _dbContext.SuKiens
            .Include(s => s.ThanhVien)
            .Where(s => s.NgayXayRa >= targetDate && s.NgayXayRa < nextDay)
            .ToListAsync(cancellationToken);

        if (upcomingEvents.Count == 0)
        {
            _logger.LogInformation("✅ No events found for {TargetDate:dd/MM/yyyy}. Nothing to send.", targetDate);
            return;
        }

        _logger.LogInformation("📋 Found {Count} event(s) on {TargetDate:dd/MM/yyyy}. Preparing emails...",
            upcomingEvents.Count, targetDate);

        foreach (var suKien in upcomingEvents)
        {
            await ProcessSingleEventAsync(suKien, targetDate, cancellationToken);
        }
    }

    private async Task ProcessSingleEventAsync(
        GiaPha_Domain.Entities.SuKien suKien,
        DateTime targetDate,
        CancellationToken ct)
    {
        try
        {
            var thanhVien = suKien.ThanhVien;
            if (thanhVien == null || thanhVien.HoId == null)
            {
                _logger.LogWarning("⚠️ SuKien {Id} - ThanhVien or HoId is null, skipping.", suKien.Id);
                return;
            }

            var hoId = thanhVien.HoId.Value;

            // Lấy tất cả email của tài khoản người dùng thuộc cùng dòng họ
            var memberEmails = await GetMemberEmailsByHoIdAsync(hoId, ct);

            if (memberEmails.Count == 0)
            {
                _logger.LogWarning("⚠️ No member emails found for HoId {HoId}, skipping event {EventId}.",
                    hoId, suKien.Id);
                return;
            }

            // Tạo nội dung email và gửi
            var subject = $"🔔 Nhắc nhở: Sự kiện \"{suKien.LoaiSuKien}\" sẽ diễn ra sau 3 ngày nữa!";
            var htmlBody = BuildEmailHtml(suKien, thanhVien.HoTen, targetDate);

            var success = await _emailService.SendEmailAsync(memberEmails, subject, htmlBody, isHtml: true);

            if (success)
            {
                _logger.LogInformation(
                    "✅ Email sent to {Count} members for event \"{Event}\" on {Date:dd/MM/yyyy}.",
                    memberEmails.Count, suKien.LoaiSuKien, targetDate);
            }
            else
            {
                _logger.LogWarning("⚠️ Failed to send email for event \"{Event}\".", suKien.LoaiSuKien);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error processing event {EventId}", suKien.Id);
        }
    }

    private async Task<List<string>> GetMemberEmailsByHoIdAsync(Guid hoId, CancellationToken ct)
    {
        return await _dbContext.Set<GiaPha_Domain.Entities.TaiKhoan_Ho>()
            .Where(th => th.HoId == hoId)
            .Include(th => th.TaiKhoan)
            .Select(th => th.TaiKhoan.Email)
            .Where(email => !string.IsNullOrEmpty(email))
            .Distinct()
            .ToListAsync(ct);
    }

    /// <summary>
    /// Tạo nội dung email HTML đẹp mắt cho thông báo sự kiện
    /// </summary>
    private static string BuildEmailHtml(
        GiaPha_Domain.Entities.SuKien suKien,
        string tenThanhVien,
        DateTime ngayDienRa)
    {
        var diaDiem = string.IsNullOrWhiteSpace(suKien.DiaDiem) ? "Chưa cập nhật" : suKien.DiaDiem;
        var moTa = string.IsNullOrWhiteSpace(suKien.MoTa) ? "Không có mô tả chi tiết." : suKien.MoTa;
        var ngayFormatted = ngayDienRa.ToString("dddd, dd/MM/yyyy");

        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
</head>
<body style=""margin:0; padding:0; background-color:#f0f4f8; font-family:'Segoe UI',Arial,sans-serif;"">
    <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#f0f4f8; padding:40px 20px;"">
        <tr>
            <td align=""center"">
                <table width=""600"" cellpadding=""0"" cellspacing=""0"" style=""background:#ffffff; border-radius:16px; overflow:hidden; box-shadow:0 4px 24px rgba(0,0,0,0.08);"">
                    
                    <!-- Header -->
                    <tr>
                        <td style=""background:linear-gradient(135deg,#2563eb,#7c3aed); padding:32px 40px; text-align:center;"">
                            <h1 style=""color:#fff; margin:0; font-size:24px; font-weight:700;"">🔔 Nhắc Nhở Sự Kiện</h1>
                            <p style=""color:rgba(255,255,255,0.85); margin:8px 0 0; font-size:14px;"">Gia Phả Dòng Họ - Thông báo tự động</p>
                        </td>
                    </tr>

                    <!-- Body -->
                    <tr>
                        <td style=""padding:32px 40px;"">
                            <p style=""color:#334155; font-size:16px; line-height:1.6; margin:0 0 24px;"">
                                Xin chào quý bà con,<br>
                                Sự kiện sau đây sẽ diễn ra trong <strong style=""color:#2563eb;"">3 ngày nữa</strong>. Kính mời mọi người chuẩn bị và sắp xếp tham gia.
                            </p>

                            <!-- Event Card -->
                            <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background:#f8fafc; border:1px solid #e2e8f0; border-radius:12px; overflow:hidden;"">
                                <tr>
                                    <td style=""height:6px; background:linear-gradient(90deg,#3b82f6,#8b5cf6);""></td>
                                </tr>
                                <tr>
                                    <td style=""padding:24px;"">
                                        <h2 style=""color:#1e293b; margin:0 0 16px; font-size:20px;"">{suKien.LoaiSuKien}</h2>
                                        
                                        <table cellpadding=""0"" cellspacing=""0"" style=""width:100%;"">
                                            <tr>
                                                <td style=""padding:8px 0; color:#64748b; font-size:14px; width:140px; vertical-align:top;"">📅 Ngày diễn ra:</td>
                                                <td style=""padding:8px 0; color:#1e293b; font-size:14px; font-weight:600;"">{ngayFormatted}</td>
                                            </tr>
                                            <tr>
                                                <td style=""padding:8px 0; color:#64748b; font-size:14px; vertical-align:top;"">📍 Địa điểm:</td>
                                                <td style=""padding:8px 0; color:#1e293b; font-size:14px; font-weight:600;"">{diaDiem}</td>
                                            </tr>
                                            <tr>
                                                <td style=""padding:8px 0; color:#64748b; font-size:14px; vertical-align:top;"">👤 Thành viên:</td>
                                                <td style=""padding:8px 0; color:#1e293b; font-size:14px; font-weight:600;"">{tenThanhVien}</td>
                                            </tr>
                                            <tr>
                                                <td style=""padding:8px 0; color:#64748b; font-size:14px; vertical-align:top;"">📝 Mô tả:</td>
                                                <td style=""padding:8px 0; color:#475569; font-size:14px;"">{moTa}</td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>

                            <p style=""color:#64748b; font-size:13px; margin:24px 0 0; line-height:1.5;"">
                                Đây là email tự động từ hệ thống Gia Phả Dòng Họ. 
                                Nếu bạn có thắc mắc, vui lòng liên hệ Trưởng họ.
                            </p>
                        </td>
                    </tr>

                    <!-- Footer -->
                    <tr>
                        <td style=""background:#f8fafc; padding:20px 40px; border-top:1px solid #e2e8f0; text-align:center;"">
                            <p style=""color:#94a3b8; font-size:12px; margin:0;"">© {DateTime.UtcNow.Year} Gia Phả Dòng Họ. Bảo lưu mọi quyền.</p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
    }
}
