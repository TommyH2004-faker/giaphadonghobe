
using GiaPha_Application.Features.QuanheChaMe.Command.Create;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GiaPha_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuanHeChaConController : ControllerBase
{
    private readonly IMediator _mediator;

    public QuanHeChaConController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateQuanHeChaCon([FromBody] CreateQuanHeChaConCommand command)
    {
        var result = await _mediator.Send(command);
        
        if (!result.IsSuccess)
            return BadRequest(result);
        
        return Ok(result);
    }
}