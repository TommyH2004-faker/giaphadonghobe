using GiaPha_Application.Features.HonNhan.Command.Create;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GiaPha_WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HonNhanController : ControllerBase
{
    private readonly IMediator _mediator;

    public HonNhanController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateHonNhan([FromBody] CreateHonNhanCommand command)
    {
        var result = await _mediator.Send(command);
        
        if (!result.IsSuccess)
            return BadRequest(result);
        
        return Ok(result);
    }
}