
using GiaPha_Application.Common;
using GiaPha_Application.DTOs;
using GiaPha_Application.Features.HoName.Command.CreateHo;
using GiaPha_Application.Features.HoName.Command.UpdateHo;
using GiaPha_Application.Features.HoName.Queries.GetById;

using GiaPha_Application.Features.HoName.Queries.GetMyHos;
using GiaPha_Application.Features.HoName.Queries.GetHoByUserEmail;
using GiaPha_Application.Features.HoName.Queries.GetHoByTruongHoEmail;

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static GiaPha_WebAPI.Controller.ControllerHo.RequestHo;

namespace GiaPha_WebAPI.Controller.ControllerHo;

[ApiController]
[Route("api/[controller]")]
public class HoController : ControllerBase
{
    private readonly IMediator _mediator;
    public HoController(IMediator mediator)
    {
        _mediator = mediator;
    }
    [HttpPost]
    public async Task<IActionResult> CreateHo([FromBody] CreateHoRequest request)
    {
        var command = new CreateHoCommand
        {
            UserId = request.UserId,
            TenHo = request.TenHo,
            MoTa = request.MoTa,
            QueQuan = request.QueQuan,
            HoTenThuyTo = request.HoTenThuyTo,
            GioiTinhThuyTo = request.GioiTinhThuyTo,
            NgaySinhThuyTo = request.NgaySinhThuyTo,
            NoiSinhThuyTo = request.NoiSinhThuyTo,
            TieuSuThuyTo = request.TieuSuThuyTo
        };
        var result = await _mediator.Send(command);
        return Ok(result);
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateHo(Guid id, [FromBody] UpdateHoRequest request)
    {
        var command = new UpdateHoCommand
        {
            Id = id,
            ThuyToId = request.ThuyToId,
            TenHo = request.TenHo,
            MoTa = request.MoTa
        };
        var result = await _mediator.Send(command);
        return Ok(result);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetHoById(Guid id)
    {
        var query = new GetHoByIdQuery { Id = id };
        var result = await _mediator.Send(query);
        return Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetAllHo()
    {
        var query = new GiaPha_Application.Features.HoName.Queries.GetAllHo.GetAllHoQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Lấy thông tin họ mà user hiện tại đang tham gia (từ JWT token)
    /// </summary>
    [HttpGet("my")]
    [Authorize]
    public async Task<IActionResult> GetMyHos()
    {
        // Lấy currentHoId từ JWT token
        var hoIdClaim = User.FindFirst("currentHoId")?.Value;
        if (string.IsNullOrEmpty(hoIdClaim) || !Guid.TryParse(hoIdClaim, out var hoId))
        {
            return Ok(Result<List<HoResponse>>.Success(new List<HoResponse>()));
        }

        var query = new GetMyHosQuery(hoId);
        var result = await _mediator.Send(query);
        
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Lấy thông tin họ theo email của trưởng họ
    /// </summary>
    [HttpGet("by-email/{email}")]
    public async Task<IActionResult> GetHoByUserEmail(string email)
    {
        var query = new GetHoByUserEmailQuery(email);
        var result = await _mediator.Send(query);
        
        if (!result.IsSuccess)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Lấy thông tin họ theo email Trưởng họ (dùng cho xin vào họ)
    /// </summary>
    [HttpGet("by-truongho-email")]
    [Authorize]
    public async Task<IActionResult> GetHoByTruongHoEmail([FromQuery] string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return BadRequest(Result<object>.Failure(ErrorType.Validation, "Email không được để trống"));
        }

        var query = new GetHoByTruongHoEmailQuery(email.Trim());
        var result = await _mediator.Send(query);
        
        if (!result.IsSuccess)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    [HttpGet("top3")]
    public async Task<IActionResult> GetTop3Ho()
    {
        var query = new GetTop3HoQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }


    /// <summary>
 

    [HttpPost("{id}/assign-thuy-to/{thuyToId}")]
    public async Task<IActionResult> AssignThuyTo(Guid id,  Guid thuyToId)
    {
        var command = new GiaPha_Application.Features.HoName.Command.AssignThuyTo.AssignThuyToCommand
        {
            HoId = id,
            ThuyToId = thuyToId
        };
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}