using FlexTravel.Application.UseCases.Commands;
using FlexTravel.Implementation;
using Microsoft.AspNetCore.Mvc;

namespace FlexTravel.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InitialController : ControllerBase
    {
        private readonly UseCaseHandler _useCaseHandler;
        private readonly IWebHostEnvironment _environment;

        public InitialController(UseCaseHandler handler, IWebHostEnvironment environment)
        {
            _useCaseHandler = handler;
            _environment = environment;
        }

        [HttpPost]
        public IActionResult Post([FromServices] IFillDatabaseCommand command)
        {
            if (!_environment.IsDevelopment())
            {
                return NotFound();
            }

            _useCaseHandler.HandleCommand(command, 0);
            return StatusCode(StatusCodes.Status201Created);
        }
    }
}
