using GiaPha_Application.Features.ThanhVien.Command.Create;
using GiaPha_Application.Features.ThanhVien.Command.Update;
using GiaPha_Application.Features.ThanhVien.Command.UpdateAvatar;
using GiaPha_Application.Features.ThanhVien.Command.Delete;
using GiaPha_Application.Features.ThanhVien.Queries.GetById;
using GiaPha_Application.Service;
using GiaPha_Application.Repository;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using GiaPha_Application.Features.ThanhVien.Queries.GetByHoid;

namespace GiaPha_WebAPI.Controller.ThanhVienController;

[ApiController]
[Route("api/[controller]")]
public class ThanhVienController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICloudinaryService _cloudinaryService;
    private readonly ILogger<ThanhVienController> _logger;
    private readonly IAuthRepository _authRepository;
    private readonly IThanhVienRepository _thanhVienRepository;

    public ThanhVienController(
        IMediator mediator, 
        ICloudinaryService cloudinaryService, 
        ILogger<ThanhVienController> logger,
        IAuthRepository authRepository,
        IThanhVienRepository thanhVienRepository)
    {
        _mediator = mediator;
        _cloudinaryService = cloudinaryService;
        _logger = logger;
        _authRepository = authRepository;
        _thanhVienRepository = thanhVienRepository;
    }

    /// <summary>
    /// Tạo thành viên mới (chỉ Trưởng họ)
    /// Cho phép tạo vợ/chồng từ họ khác (HoId = null/empty)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create(CreateThanhVienCommand command)
    {
        // Lấy userId từ JWT
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized(new { message = "Token không hợp lệ" });
        }

        // Kiểm tra quyền:
        // - Nếu HoId null/empty (vợ/chồng từ họ khác): chỉ cần đăng nhập
        // - Nếu HoId có giá trị: phải là Trưởng họ
        if (command.HoId.HasValue && command.HoId.Value != Guid.Empty)
        {
            var hasPermission = await CheckTruongHoPermission(userId, command.HoId.Value);
            if (!hasPermission)
            {
                return Forbid(); // 403 Forbidden
            }
        }

        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);
        return Ok(result.Data);
    }

    /// <summary>
    /// Lấy thông tin thành viên (thành viên cùng họ có thể xem)
    /// </summary>
    ///
    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        // Lấy userId từ JWT
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized(new { message = "Token không hợp lệ" });
        }

        // Lấy thông tin thành viên để biết họ nào
        var memberResult = await _thanhVienRepository.GetThanhVienByIdAsync(id);
        if (!memberResult.IsSuccess || memberResult.Data == null)
        {
            return NotFound(new { message = "Không tìm thấy thành viên" });
        }

        // Kiểm tra user có thuộc cùng họ không (hoặc là Admin)
        var isAdmin = User.IsInRole("ADMIN");
        var isSameHo = await CheckSameHo(userId, memberResult.Data.HoId ?? Guid.Empty);
        
        if (!isSameHo && !isAdmin)
        {
            return Forbid(); // 403 Forbidden
        }

        var query = new GetThanhVienByIdQuery(id);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return NotFound(result);
        
        return Ok(result.Data);
    }

    /// <summary>
    /// Cập nhật thông tin thành viên (chỉ Trưởng họ)
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateThanhVienCommand command)
    {
        // Lấy userId từ JWT
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized(new { message = "Token không hợp lệ" });
        }

        // Lấy thông tin thành viên để biết họ nào
        var memberResult = await _thanhVienRepository.GetThanhVienByIdAsync(id);
        if (!memberResult.IsSuccess || memberResult.Data == null)
        {
            return NotFound(new { message = "Không tìm thấy thành viên" });
        }

        // Kiểm tra quyền: chỉ Trưởng họ mới được sửa
        // Nếu thành viên không thuộc họ nào (vợ/chồng từ họ khác, HoId = null),
        // dùng currentHoId từ JWT token để xác định họ mà người dùng đang quản lý
        Guid hoIdToCheck = memberResult.Data.HoId ?? Guid.Empty;
        if (hoIdToCheck == Guid.Empty)
        {
            var currentHoIdClaim = User.FindFirst("currentHoId");
            if (currentHoIdClaim != null && Guid.TryParse(currentHoIdClaim.Value, out var currentHoId))
            {
                hoIdToCheck = currentHoId;
            }
        }

        var hasPermission = await CheckTruongHoPermission(userId, hoIdToCheck);
        if (!hasPermission)
        {
            return Forbid(); // 403 Forbidden
        }

        // Ensure the ID in the route matches the command
        var commandWithId = command with { Id = id };
        var result = await _mediator.Send(commandWithId);

        if (!result.IsSuccess)
            return BadRequest(result);
        
        return Ok(result.Data);
    }

    /// <summary>
    /// Xóa thành viên (chỉ Trưởng họ)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            // Lấy userId từ JWT
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                _logger.LogWarning("Invalid token or userId claim");
                return Unauthorized(new { message = "Token không hợp lệ" });
            }

            _logger.LogInformation("🗑️ Delete request - MemberId: {Id}, UserId: {UserId}", id, userId);

            // Lấy thông tin thành viên để biết họ nào
            // ⚠️ Sử dụng IgnoreQueryFilters để có thể xóa người đã bị xóa trước đó
            var memberResult = await _thanhVienRepository.GetThanhVienByIdWithDeletedAsync(id);
            if (!memberResult.IsSuccess || memberResult.Data == null)
            {
                _logger.LogWarning("Member not found: {Id}", id);
                return NotFound(new { message = "Không tìm thấy thành viên" });
            }

            _logger.LogInformation("Member found - HoId: {HoId}", memberResult.Data.HoId);

            // Kiểm tra quyền: chỉ Trưởng họ mới được xóa
            // Nếu thành viên không thuộc họ nào (vợ/chồng từ họ khác, HoId = null),
            // dùng currentHoId từ JWT token để xác định họ mà người dùng đang quản lý
            Guid hoIdToCheck = memberResult.Data.HoId ?? Guid.Empty;
            if (hoIdToCheck == Guid.Empty)
            {
                var currentHoIdClaim = User.FindFirst("currentHoId");
                if (currentHoIdClaim != null && Guid.TryParse(currentHoIdClaim.Value, out var currentHoId))
                {
                    hoIdToCheck = currentHoId;
                }
            }

            var hasPermission = await CheckTruongHoPermission(userId, hoIdToCheck);
            _logger.LogInformation("Permission check result: {HasPermission}", hasPermission);
            
            if (!hasPermission)
            {
                _logger.LogWarning("User {UserId} does not have permission to delete member {MemberId}", userId, id);
                return Forbid(); // 403 Forbidden
            }

            var command = new DeleteThanhVienCommand(id);
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                _logger.LogError("Delete command failed: {ErrorMessage}", result.ErrorMessage);
                return BadRequest(result);
            }
            
            _logger.LogWarning("✅ User {UserId} deleted member {MemberId}", userId, id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error deleting member {MemberId}", id);
            return StatusCode(500, new 
            { 
                IsSuccess = false, 
                ErrorMessage = ex.Message,
                StackTrace = ex.StackTrace 
            });
        }
    }

    /// <summary>
    /// Upload avatar (chỉ Trưởng họ)
    /// </summary>
    [HttpPost("{id}/avatar")]
    public async Task<IActionResult> UploadAvatar(Guid id, [FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { IsSuccess = false, ErrorMessage = "Không có file được chọn" });

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
            return BadRequest(new { IsSuccess = false, ErrorMessage = "Chỉ chấp nhận file ảnh" });

        if (file.Length > 5 * 1024 * 1024)
            return BadRequest(new { IsSuccess = false, ErrorMessage = "File không vượt quá 5MB" });

        // Lấy userId từ JWT
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized(new { message = "Token không hợp lệ" });
        }

        // Lấy thông tin thành viên để biết họ nào
        var memberResult = await _thanhVienRepository.GetThanhVienByIdAsync(id);
        if (!memberResult.IsSuccess || memberResult.Data == null)
        {
            return NotFound(new { message = "Không tìm thấy thành viên" });
        }

        // Kiểm tra quyền: chỉ Trưởng họ mới được upload avatar
        // Nếu thành viên không thuộc họ nào (vợ/chồng từ họ khác, HoId = null),
        // dùng currentHoId từ JWT token để xác định họ mà người dùng đang quản lý
        Guid hoIdToCheck = memberResult.Data.HoId ?? Guid.Empty;
        if (hoIdToCheck == Guid.Empty)
        {
            var currentHoIdClaim = User.FindFirst("currentHoId");
            if (currentHoIdClaim != null && Guid.TryParse(currentHoIdClaim.Value, out var currentHoId))
            {
                hoIdToCheck = currentHoId;
            }
        }

        var hasPermission = await CheckTruongHoPermission(userId, hoIdToCheck);
        if (!hasPermission)
        {
            return Forbid(); // 403 Forbidden
        }

        try
        {
            _logger.LogInformation("🖼️ Upload avatar request - ThanhVienId: {Id}, FileName: {FileName}, Size: {Size}KB", 
                id, file.FileName, file.Length / 1024);

            // Upload to Cloudinary
            var fileName = $"avatar_{id}_{Guid.NewGuid()}";
            string avatarUrl;

            using (var stream = file.OpenReadStream())
            {
                avatarUrl = await _cloudinaryService.UploadImageAsync(stream, fileName, "giapha/avatars");
            }

            _logger.LogInformation("✅ Cloudinary upload successful - URL: {Url}", avatarUrl);

            // Update database
            var command = new UpdateAvatarCommand
            {
                ThanhVienId = id,
                AvatarUrl = avatarUrl
            };

            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("⚠️ Database update failed after Cloudinary upload");
                // TODO: Delete uploaded image from Cloudinary if DB update fails
                // await _cloudinaryService.DeleteImageAsync($"giapha/avatars/{fileName}");
                return BadRequest(result);
            }

            _logger.LogInformation("✅ Avatar updated successfully for ThanhVien: {Id}", id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error uploading avatar for ThanhVien: {Id}", id);
            return StatusCode(500, new
            {
                IsSuccess = false,
                ErrorMessage = ex.Message,
                StackTrace = ex.StackTrace
            });
        }
    }

    // ==================== HELPER METHODS ====================

    /// <summary>
    /// Kiểm tra user có phải Trưởng họ của họ này không
    /// </summary>
    private async Task<bool> CheckTruongHoPermission(Guid userId, Guid hoId)
    {
        try
        {
            _logger.LogInformation("Checking permission - UserId: {UserId}, HoId: {HoId}", userId, hoId);

            // Admin có quyền tuyệt đối
            if (User.IsInRole("ADMIN"))
            {
                _logger.LogInformation("User is ADMIN - permission granted");
                return true;
            }

            var userResult = await _authRepository.GetUserByIdAsync(userId);
            if (userResult == null)
            {
                _logger.LogWarning("User not found: {UserId}", userId);
                return false;
            }

            var user = userResult;
            _logger.LogInformation("User loaded - TaiKhoan_Hos count: {Count}", user.TaiKhoan_Hos?.Count ?? 0);

            if (user.TaiKhoan_Hos == null || !user.TaiKhoan_Hos.Any())
            {
                _logger.LogWarning("User has no TaiKhoan_Hos");
                return false;
            }

            var taiKhoanHo = user.TaiKhoan_Hos.FirstOrDefault(th => th.HoId == hoId);
            
            if (taiKhoanHo == null)
            {
                _logger.LogWarning("User not member of Ho {HoId}", hoId);
                return false;
            }

            var roleInHo = (int)taiKhoanHo.RoleInHo;
            _logger.LogInformation("User role in Ho: {RoleInHo} (0=TruongHo, 1=ThanhVien)", roleInHo);

            // RoleInHo: 0 = Trưởng họ, 1 = Thành viên
            return roleInHo == 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking permission for UserId: {UserId}, HoId: {HoId}", userId, hoId);
            return false;
        }
    }

    /// <summary>
    /// Kiểm tra user có thuộc họ này không
    /// </summary>
    private async Task<bool> CheckSameHo(Guid userId, Guid hoId)
    {
        var userResult = await _authRepository.GetUserByIdAsync(userId);
        if (userResult == null)
        {
            return false;
        }

        var user = userResult;
        return user.TaiKhoan_Hos.Any(th => th.HoId == hoId);
    }
   [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetThanhVien()
    {
        var hoIdClaim = User.FindFirst("currentHoId")?.Value;

        if (string.IsNullOrEmpty(hoIdClaim))
            return Unauthorized("Không tìm thấy HoId trong token.");

        if (!Guid.TryParse(hoIdClaim, out var hoId))
            return BadRequest("HoId không hợp lệ.");

        var query = new GetThanhVienByHoIdQuery(hoId);
        var result = await _mediator.Send(query);

        return Ok(result);
    }
} 