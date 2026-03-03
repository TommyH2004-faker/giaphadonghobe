using GiaPha_Application.Features.GiaPha.Queries.GetMyGiaPhaTree;
using GiaPha_Application.Repository;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GiaPha_WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GiaPhaController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IAuthRepository _authRepository;

    public GiaPhaController(IMediator mediator, IAuthRepository authRepository)
    {
        _mediator = mediator;
        _authRepository = authRepository;
    }

    /// <summary>
    /// Lấy gia phả của user hiện tại (yêu cầu đăng nhập)
    /// </summary>
    [Authorize]
    [HttpGet("my-tree")]
    public async Task<IActionResult> GetMyGiaPhaTree(
        [FromQuery] Guid? hoId = null,
        [FromQuery] int maxLevel = 10,
        [FromQuery] bool includeNuGioi = true,
        [FromQuery] bool includeDeleted = true)
    {
        // Lấy userId từ JWT token
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized(new { message = "Token không hợp lệ" });
        }

        // Nếu không có hoId, lấy ho đầu tiên của user
        if (!hoId.HasValue || hoId.Value == Guid.Empty)
        {
            var userResult = await _authRepository.GetUserByIdAsync(userId);
            
            if ( userResult== null)
            {
                return NotFound(new { 
                    isSuccess = false,
                    errorMessage = "Không tìm thấy người dùng" 
                });
            }

            var user = userResult;
            var firstHo = user.TaiKhoan_Hos?.FirstOrDefault();
            
            if (firstHo == null)
            {
                return NotFound(new { 
                    isSuccess = false,
                    errorMessage = "Bạn chưa thuộc họ nào. Vui lòng tạo họ mới hoặc liên hệ admin để được thêm vào họ." 
                });
            }

            hoId = firstHo.HoId;
        }

        var query = new GetMyGiaPhaTreeQuery
        {
            UserId = userId,
            HoId = hoId.Value,
            MaxLevel = maxLevel,
            IncludeNuGioi = includeNuGioi,
            IncludeDeleted = includeDeleted
        };

        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
        {
            // Nếu user chưa có họ, trả về 404 với message
            if (result.ErrorType == GiaPha_Application.Common.ErrorType.NotFound)
                return NotFound(result);

            return BadRequest(result);
        }

        return Ok(result);
    }
}