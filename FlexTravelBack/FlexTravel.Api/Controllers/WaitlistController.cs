using FlexTravel.Application.Dto;
using FlexTravel.Application.UseCases.Commands;
using FlexTravel.Application.UseCases.Queries;
using FlexTravel.Implementation;
using Microsoft.AspNetCore.Mvc;

namespace FlexTravel.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WaitlistController : ControllerBase
    {
        private readonly UseCaseHandler _useCaseHandler;

        public WaitlistController(UseCaseHandler handler)
        {
            _useCaseHandler = handler;
        }

        [HttpGet("mine")]
        public IActionResult Mine([FromServices] IGetMyWaitlistQuery query)
        {
            return Ok(_useCaseHandler.HandleQuery(query, 0));
        }

        [HttpPost]
        public IActionResult Join([FromBody] JoinWaitlistDto request, [FromServices] IJoinWaitlistCommand command)
        {
            var id = _useCaseHandler.HandleCommand(command, request);
            return StatusCode(StatusCodes.Status201Created, new { id });
        }

        [HttpDelete("{id:int}")]
        public IActionResult Leave(int id, [FromServices] ILeaveWaitlistCommand command)
        {
            _useCaseHandler.HandleCommand(command, id);
            return NoContent();
        }
    }
}
