

using GiaPha_Application.Features.TaiKhoanNguoiDungs.Command.UpdateAvatar;
using GiaPha_Application.Features.TaiKhoanNguoiDungs.Queries.GetTaiKhoanNguoiDungByid;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GiaPha_WebAPI.Controller;
[ApiController]
[Route("api/[controller]")]
public class TaiKhoanNguoiDung : ControllerBase
{
    private readonly IMediator _mediator;

    public TaiKhoanNguoiDung(IMediator mediator)
    {
        _mediator = mediator;
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTaiKhoanNguoiDungById(Guid id)
    {
        var query = new GetTaiKhoanNguoiDungQueryById(id);
        var result = await _mediator.Send(query);
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return NotFound(result);
    }
    [HttpPost("update-avatar")]
    public async Task<IActionResult> UpdateAvatar([FromBody] UpdateAvatarCommand command)
    {
        var result = await _mediator.Send(command);
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }
}