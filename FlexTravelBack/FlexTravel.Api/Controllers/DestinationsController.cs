using FlexTravel.Application.Dto;
using FlexTravel.Application.UseCases.Queries;
using FlexTravel.Implementation;
using Microsoft.AspNetCore.Mvc;

namespace FlexTravel.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DestinationsController : ControllerBase
    {
        private readonly UseCaseHandler _useCaseHandler;

        public DestinationsController(UseCaseHandler handler)
        {
            _useCaseHandler = handler;
        }

        [HttpGet]
        public IActionResult Get([FromQuery] DestinationSearchDto search, [FromServices] IGetDestinationsQuery query)
        {
            return Ok(_useCaseHandler.HandleQuery(query, search));
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id, [FromServices] IGetDestinationQuery query)
        {
            return Ok(_useCaseHandler.HandleQuery(query, id));
        }
    }
}
