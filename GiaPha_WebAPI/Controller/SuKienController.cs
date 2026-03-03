using GiaPha_Application.Features.SuKien.Command.Create;
using GiaPha_Application.Features.SuKien.Command.Update;
using GiaPha_Application.Features.SuKien.Queries.GetAllSuKien;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GiaPha_WebAPI.Controller;
[ApiController]
[Route("api/[controller]")]
public class SuKienController : ControllerBase
{
    private readonly IMediator Mediator;
    public SuKienController(IMediator mediator)
    {
        Mediator = mediator;
    }
    [HttpPost]
    public async Task<IActionResult> CreateSuKien([FromBody] CreateSuKienCommand command)
    {
        if (command == null)
            return BadRequest("Command null từ swagger");

        var result = await Mediator.Send(command);
        return Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetAllSuKien()
    {
       var query = new GetAllSuKienQuery();
       var result = await Mediator.Send(query);
         return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSuKien(Guid id, [FromBody] UpdateSuKienCommand command)
    {
        if (command == null)
            return BadRequest("Command null");

        var commandWithId = command with { Id = id };
        var result = await Mediator.Send(commandWithId);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }
}