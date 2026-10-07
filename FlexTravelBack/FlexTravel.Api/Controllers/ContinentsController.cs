using FlexTravel.Application.UseCases.Queries;
using FlexTravel.Implementation;
using Microsoft.AspNetCore.Mvc;

namespace FlexTravel.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContinentsController : ControllerBase
    {
        private readonly UseCaseHandler _useCaseHandler;

        public ContinentsController(UseCaseHandler handler)
        {
            _useCaseHandler = handler;
        }

        [HttpGet]
        public IActionResult Get([FromServices] IGetContinentsQuery query)
        {
            return Ok(_useCaseHandler.HandleQuery(query, 0));
        }
    }
}
