using GiaPha_Application.Features.YeuCau.Commands.DuyetYeuCau;
using GiaPha_Application.Features.YeuCau.Commands.TuChoiYeuCau;
using GiaPha_Application.Features.YeuCau.Queries.GetPendingRequests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GiaPha_WebAPI.Controller;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class YeuCauController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<YeuCauController> _logger;

    public YeuCauController(IMediator mediator, ILogger<YeuCauController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// 📋 Trưởng họ xem danh sách yêu cầu đang chờ duyệt
    /// </summary>
    [HttpGet("pending")]
    public async Task<IActionResult> GetPendingRequests()
    {
        var hoIdClaim = User.FindFirst("currentHoId")?.Value;
        if (string.IsNullOrEmpty(hoIdClaim) || !Guid.TryParse(hoIdClaim, out var hoId))
            return BadRequest("Bạn chưa chọn dòng họ");

        // Chỉ Trưởng họ được xem
        var roleInHo = User.FindFirst("roleInHo")?.Value;
        if (roleInHo != "0")
            return StatusCode(403, "Chỉ Trưởng họ mới có quyền xem danh sách yêu cầu");

        var query = new GetPendingRequestsQuery(hoId);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result.Data);
    }

    /// <summary>
    /// ✅ Trưởng họ duyệt yêu cầu tham gia
    /// </summary>
    [HttpPut("{yeuCauId}/duyet")]
    public async Task<IActionResult> DuyetYeuCau(Guid yeuCauId)
    {
        var roleInHo = User.FindFirst("roleInHo")?.Value;
        if (roleInHo != "0")
            return StatusCode(403, "Chỉ Trưởng họ mới có quyền duyệt");

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var command = new DuyetYeuCauCommand(yeuCauId, userId);
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(new { message = "Đã duyệt yêu cầu thành công, thành viên đã được thêm vào họ" });
    }

    /// <summary>
    /// ❌ Trưởng họ từ chối yêu cầu tham gia
    /// </summary>
    [HttpPut("{yeuCauId}/tu-choi")]
    public async Task<IActionResult> TuChoiYeuCau(Guid yeuCauId, [FromBody] TuChoiRequest request)
    {
        var roleInHo = User.FindFirst("roleInHo")?.Value;
        if (roleInHo != "0")
            return StatusCode(403, "Chỉ Trưởng họ mới có quyền từ chối");

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var command = new TuChoiYeuCauCommand(yeuCauId, userId, request.GhiChu);
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(new { message = "Đã từ chối yêu cầu" });
    }

    /// <summary>
    /// 📝 User gửi yêu cầu xin vào họ
    /// </summary>
    [HttpPost("xin-vao/{hoId}")]
    public async Task<IActionResult> XinVaoHo(Guid hoId, [FromBody] XinVaoHoRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var command = new GiaPha_Application.Features.YeuCau.Commands.XinVaoHo.XinVaoHoCommand(userId, hoId, request.LyDoXinVao);
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(new { message = "Đã gửi yêu cầu, vui lòng chờ Trưởng họ phê duyệt" });
    }
}

public record TuChoiRequest(string? GhiChu);
public record XinVaoHoRequest(string LyDoXinVao);
