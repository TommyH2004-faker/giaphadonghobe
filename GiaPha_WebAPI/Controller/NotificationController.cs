using GiaPha_Application.Features.Notification.Commands.CreateNotification;
using GiaPha_Application.Features.Notification.Commands.MarkAsRead;
using GiaPha_Application.Features.Notification.Queries.GetMyNotifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GiaPha_WebAPI.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<NotificationController> _logger;

        public NotificationController(
            IMediator mediator,
            ILogger<NotificationController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// 🔔 Lấy tất cả notifications của user hiện tại
        /// </summary>
        [HttpGet("my")]
        public async Task<IActionResult> GetMyNotifications()
        {
            // 1️⃣ Lấy userId từ JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Không tìm thấy thông tin user trong token");
            }

            // 2️⃣ Lấy currentHoId từ JWT claims
            var hoIdClaim = User.FindFirst("currentHoId")?.Value;
            Guid? hoId = null;
            if (!string.IsNullOrEmpty(hoIdClaim) && Guid.TryParse(hoIdClaim, out var parsedHoId))
            {
                hoId = parsedHoId;
            }

            // 3️⃣ Gửi query qua MediatR
            var query = new GetMyNotificationsQuery(userId, hoId);
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// ✅ Đánh dấu notification đã đọc
        /// </summary>
        [HttpPut("read/{notificationId}")]
        public async Task<IActionResult> MarkAsRead(Guid notificationId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Không tìm thấy thông tin user trong token");
            }

            var command = new MarkAsReadCommand(notificationId, userId);
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return NoContent();
        }

        /// <summary>
        /// ✍️ Tạo thông báo cho toàn bộ dòng họ (chỉ Trưởng họ)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateNotification([FromBody] CreateNotificationRequest request)
        {
            // Lấy userId từ JWT
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized("Không tìm thấy thông tin user trong token");

            // Lấy currentHoId từ JWT
            var hoIdClaim = User.FindFirst("currentHoId")?.Value;
            if (string.IsNullOrEmpty(hoIdClaim) || !Guid.TryParse(hoIdClaim, out var hoId))
                return BadRequest("Bạn chưa chọn dòng họ");

            // Kiểm tra role từ JWT claim (0 = Trưởng họ)
            var roleInHoClaim = User.FindFirst("roleInHo")?.Value;
            if (roleInHoClaim != "0")
                return StatusCode(403, "Chỉ Trưởng họ mới có quyền gửi thông báo");

            var command = new CreateNotificationCommand(userId, request.NoiDung, hoId);
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(new { notificationId = result.Data, message = "Đã gửi thông báo đến toàn bộ thành viên" });
        }
    }

    public record CreateNotificationRequest(string NoiDung);
}
