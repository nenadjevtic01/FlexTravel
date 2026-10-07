using FlexTravel.Application.Dto;
using FlexTravel.Application.UseCases.Commands;
using FlexTravel.Application.UseCases.Queries;
using FlexTravel.Implementation;
using Microsoft.AspNetCore.Mvc;

namespace FlexTravel.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly UseCaseHandler _useCaseHandler;

        public AdminController(UseCaseHandler handler)
        {
            _useCaseHandler = handler;
        }

        [HttpPut("users/{id:int}")]
        public IActionResult UpdateUser(int id, [FromBody] UpdateUserDto request, [FromServices] IUpdateUserCommand command)
        {
            request.UserId = id;
            _useCaseHandler.HandleCommand(command, request);
            return NoContent();
        }

        [HttpDelete("users/{id:int}/avatar")]
        public IActionResult RemoveAvatar(int id, [FromServices] IRemoveUserAvatarCommand command)
        {
            _useCaseHandler.HandleCommand(command, id);
            return NoContent();
        }

        [HttpGet("jobs")]
        public IActionResult Jobs([FromServices] IGetScheduledJobRunsQuery query)
        {
            return Ok(_useCaseHandler.HandleQuery(query, 0));
        }

        [HttpPost("jobs/run")]
        public IActionResult RunJobs([FromServices] IRunScheduledJobsCommand command)
        {
            return Ok(_useCaseHandler.HandleCommand(command, 0));
        }

        [HttpPut("exchange-rates/{currency}")]
        public IActionResult SetExchangeRate(string currency, [FromBody] SetExchangeRateDto request,
            [FromServices] ISetExchangeRateCommand command)
        {
            request.Currency = currency;
            _useCaseHandler.HandleCommand(command, request);
            return NoContent();
        }

        [HttpGet("stats")]
        public IActionResult Stats([FromServices] IGetAdminStatsQuery query)
        {
            return Ok(_useCaseHandler.HandleQuery(query, 0));
        }

        [HttpGet("users")]
        public IActionResult Users([FromQuery] UserSearchDto search, [FromServices] ISearchUsersQuery query)
        {
            return Ok(_useCaseHandler.HandleQuery(query, search));
        }

        [HttpGet("users/{id:int}")]
        public IActionResult GetUser(int id, [FromServices] IGetUserQuery query)
        {
            return Ok(_useCaseHandler.HandleQuery(query, id));
        }

        [HttpPut("users/{id:int}/use-cases")]
        public IActionResult UseCases(int id, [FromBody] List<int> useCaseIds, [FromServices] IUpdateUserUseCasesCommand command)
        {
            _useCaseHandler.HandleCommand(command, new UpdateUserUseCasesDto { UserId = id, UseCaseIds = useCaseIds ?? new List<int>() });
            return NoContent();
        }

        [HttpPost("users/{id:int}/block")]
        public IActionResult Block(int id, [FromServices] ISetUserActiveCommand command)
        {
            _useCaseHandler.HandleCommand(command, new SetUserActiveDto { UserId = id, IsActive = false });
            return NoContent();
        }

        [HttpPost("users/{id:int}/unblock")]
        public IActionResult Unblock(int id, [FromServices] ISetUserActiveCommand command)
        {
            _useCaseHandler.HandleCommand(command, new SetUserActiveDto { UserId = id, IsActive = true });
            return NoContent();
        }

        [HttpGet("agencies")]
        public IActionResult Agencies([FromServices] IGetAgenciesQuery query)
        {
            return Ok(_useCaseHandler.HandleQuery(query, 0));
        }

        [HttpPost("agencies")]
        public IActionResult CreateAgency([FromBody] CreateAgencyDto request, [FromServices] ICreateAgencyCommand command)
        {
            var id = _useCaseHandler.HandleCommand(command, request);
            return StatusCode(StatusCodes.Status201Created, new { id });
        }

        [HttpPost("agencies/{id:int}/employees")]
        public IActionResult CreateEmployee(int id, [FromBody] CreateAgencyEmployeeDto request, [FromServices] ICreateAgencyEmployeeCommand command)
        {
            request.AgencyId = id;
            var userId = _useCaseHandler.HandleCommand(command, request);
            return StatusCode(StatusCodes.Status201Created, new { id = userId });
        }
    }
}
