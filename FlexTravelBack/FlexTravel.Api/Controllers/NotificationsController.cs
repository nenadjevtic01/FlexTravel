using FlexTravel.Application.Dto;
using FlexTravel.Application.UseCases.Commands;
using FlexTravel.Application.UseCases.Queries;
using FlexTravel.Implementation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FlexTravel.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly UseCaseHandler _useCaseHandler;

        public NotificationsController(UseCaseHandler handler)
        {
            _useCaseHandler = handler;
        }

        [HttpGet]
        public IActionResult Get([FromServices] IGetMyNotificationsQuery query)
        {
            return Ok(_useCaseHandler.HandleQuery(query, 0));
        }

        [HttpPost("read")]
        public IActionResult Read([FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] MarkNotificationsReadDto request,
            [FromServices] IMarkNotificationsReadCommand command)
        {
            _useCaseHandler.HandleCommand(command, request ?? new MarkNotificationsReadDto());
            return NoContent();
        }
    }
}
