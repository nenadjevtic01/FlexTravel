using FlexTravel.Application.Dto;
using FlexTravel.Application.UseCases.Queries;
using FlexTravel.Implementation;
using Microsoft.AspNetCore.Mvc;

namespace FlexTravel.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripsController : ControllerBase
    {
        private readonly UseCaseHandler _useCaseHandler;

        public TripsController(UseCaseHandler handler)
        {
            _useCaseHandler = handler;
        }

        [HttpGet]
        public IActionResult Get([FromQuery] TripSearchDto search, [FromServices] ISearchTripsQuery query)
        {
            return Ok(_useCaseHandler.HandleQuery(query, search));
        }

        [HttpGet("destinations")]
        public IActionResult Destinations([FromServices] IGetTripDestinationsQuery query)
        {
            return Ok(_useCaseHandler.HandleQuery(query, 0));
        }

        [HttpGet("{id:int}")]
        public IActionResult Get(int id, [FromServices] IGetTripQuery query)
        {
            return Ok(_useCaseHandler.HandleQuery(query, id));
        }

        [HttpGet("{id:int}/similar")]
        public IActionResult Similar(int id, [FromServices] IGetSimilarTripsQuery query)
        {
            return Ok(_useCaseHandler.HandleQuery(query, id));
        }

        [HttpPost("quote")]
        public IActionResult Quote([FromBody] BookingRequestDto request, [FromServices] ICalculatePriceQuery query)
        {
            return Ok(_useCaseHandler.HandleQuery(query, request));
        }
    }
}
