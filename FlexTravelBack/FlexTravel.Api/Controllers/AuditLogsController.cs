using FlexTravel.Application.Dto;
using FlexTravel.Application.UseCases.Queries;
using FlexTravel.Implementation;
using Microsoft.AspNetCore.Mvc;

namespace FlexTravel.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditLogsController : ControllerBase
    {
        private readonly UseCaseHandler _useCaseHandler;

        public AuditLogsController(UseCaseHandler handler)
        {
            _useCaseHandler = handler;
        }

        [HttpGet]
        public IActionResult Get([FromQuery] UseCaseLogSearchDto search, [FromServices] IGetAuditLogsQuery query)
        {
            return Ok(_useCaseHandler.HandleQuery(query, search));
        }
    }
}
