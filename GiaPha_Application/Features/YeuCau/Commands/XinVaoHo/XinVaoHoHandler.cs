using GiaPha_Application.Common;
using GiaPha_Application.Repository;
using GiaPha_Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Service;

namespace GiaPha_Application.Features.YeuCau.Commands.XinVaoHo;

public class XinVaoHoHandler : IRequestHandler<XinVaoHoCommand, Result<Guid>>
{
    private readonly IYeuCauThamGiaHoRepository _repo;
    private readonly IHoRepository _hoRepo;
    private readonly IAuthRepository _authRepo;
    private readonly IEmailService _emailService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<XinVaoHoHandler> _logger;

    public XinVaoHoHandler(
        IYeuCauThamGiaHoRepository repo,
        IHoRepository hoRepo,
        IAuthRepository authRepo,
        IEmailService emailService,
        IUnitOfWork unitOfWork,
        ILogger<XinVaoHoHandler> logger)
    {
        _repo = repo;
        _hoRepo = hoRepo;
        _authRepo = authRepo;
        _emailService = emailService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(XinVaoHoCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Kiểm tra đã có yêu cầu đang chờ chưa
            var exists = await _repo.ExistsPendingAsync(request.UserId, request.HoId);
            if (exists)
                return Result<Guid>.Failure(ErrorType.Conflict, "Bạn đã có yêu cầu đang chờ duyệt cho dòng họ này");

            var yeuCau = YeuCauThamGiaHo.Create(request.UserId, request.HoId, request.LyDoXinVao);

            await _repo.AddAsync(yeuCau);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("📝 User {UserId} gửi yêu cầu vào họ {HoId}", request.UserId, request.HoId);

            // Gửi email thông báo cho Trưởng họ
            await SendEmailToTruongHo(request.UserId, request.HoId, request.LyDoXinVao);

            return Result<Guid>.Success(yeuCau.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Lỗi khi gửi yêu cầu vào họ");
            return Result<Guid>.Failure(ErrorType.InternalServerError, "Lỗi khi gửi yêu cầu");
        }
    }

    private async Task SendEmailToTruongHo(Guid userId, Guid hoId, string lyDo)
    {
        try
        {
            // Lấy email Trưởng họ
            var emailResult = await _hoRepo.GetTruongHoEmailAsync(hoId);
            if (!emailResult.IsSuccess || string.IsNullOrEmpty(emailResult.Data))
            {
                _logger.LogWarning("⚠️ Không tìm thấy email Trưởng họ {HoId}", hoId);
                return;
            }

            // Lấy thông tin người gửi yêu cầu
            var userResult = await _authRepo.GetUserByIdAsync(userId);
            if (userResult == null)
            {
                _logger.LogWarning("⚠️ Không tìm thấy thông tin user {UserId}", userId);
                return;
            }

            // Lấy thông tin họ
            var hoResult = await _hoRepo.GetHoByIdAsync(hoId);
            if (!hoResult.IsSuccess || hoResult.Data == null)
            {
                _logger.LogWarning("⚠️ Không tìm thấy thông tin họ {HoId}", hoId);
                return;
            }

            var user = userResult;
            var ho = hoResult.Data;
            var truongHoEmail = emailResult.Data;

            // Tạo nội dung email
            var subject = $"[Gia Phả] Yêu cầu tham gia Họ {ho.TenHo}";
            var body = $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 20px; border-radius: 10px 10px 0 0; }}
                        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
                        .info-box {{ background: white; padding: 15px; border-left: 4px solid #667eea; margin: 15px 0; }}
                        .button {{ display: inline-block; padding: 12px 30px; background: #667eea; color: white; text-decoration: none; border-radius: 5px; margin-top: 20px; }}
                        .footer {{ text-align: center; margin-top: 20px; color: #666; font-size: 12px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h2>🔔 Yêu cầu tham gia dòng họ</h2>
                        </div>
                        <div class='content'>
                            <p>Kính gửi Trưởng họ <strong>Họ {ho.TenHo}</strong>,</p>
                            <p>Có một yêu cầu mới xin tham gia vào dòng họ của bạn:</p>
                            
                            <div class='info-box'>
                                <p><strong>👤 Người gửi:</strong> {user.TenDangNhap}</p>
                                <p><strong>📧 Email:</strong> {user.Email}</p>
                                <p><strong>📝 Lý do:</strong></p>
                                <p style='font-style: italic; margin-left: 20px;'>{lyDo}</p>
                            </div>

                            <p>Vui lòng đăng nhập vào hệ thống để xem xét và phê duyệt yêu cầu này.</p>
                            
                            <a href='http://localhost:5173/pending-requests' class='button'>Xem danh sách yêu cầu</a>
                            
                            <div class='footer'>
                                <p>Email này được gửi tự động từ Hệ thống Quản lý Gia Phả</p>
                                <p>Vui lòng không trả lời email này</p>
                            </div>
                        </div>
                    </div>
                </body>
                </html>
            ";

            await _emailService.SendEmailAsync(truongHoEmail, subject, body, isHtml: true);
            _logger.LogInformation("📧 Đã gửi email thông báo đến Trưởng họ {Email}", truongHoEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Lỗi khi gửi email thông báo cho Trưởng họ");
            // Không throw exception để không ảnh hưởng đến việc tạo yêu cầu
        }
    }
}
