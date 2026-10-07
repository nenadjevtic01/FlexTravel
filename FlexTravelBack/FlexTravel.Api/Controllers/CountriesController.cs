using FlexTravel.Application.Dto;
using FlexTravel.Application.UseCases.Queries;
using FlexTravel.Implementation;
using Microsoft.AspNetCore.Mvc;

namespace FlexTravel.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        private readonly UseCaseHandler _useCaseHandler;

        public CountriesController(UseCaseHandler handler)
        {
            _useCaseHandler = handler;
        }

        [HttpGet]
        public IActionResult Get([FromQuery] CountrySearchDto search, [FromServices] IGetCountriesQuery query)
        {
            return Ok(_useCaseHandler.HandleQuery(query, search));
        }
    }
}
