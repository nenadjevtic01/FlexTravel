using FlexTravel.Application.Dto;
using FlexTravel.Application.UseCases.Commands;
using FlexTravel.Application.UseCases.Queries;
using FlexTravel.Implementation;
using Microsoft.AspNetCore.Mvc;

namespace FlexTravel.Api.Controllers
{
    [ApiController]
    public class PlanningController : ControllerBase
    {
        private readonly UseCaseHandler _useCaseHandler;

        public PlanningController(UseCaseHandler handler)
        {
            _useCaseHandler = handler;
        }

        [HttpGet("/api/destinations/{id:int}/weather")]
        public IActionResult Weather(int id, [FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromServices] IGetWeatherQuery query)
        {
            return Ok(_useCaseHandler.HandleQuery(query, new WeatherRequestDto { DestinationId = id, From = from, To = to }));
        }

        [HttpGet("/api/price-alerts")]
        public IActionResult PriceAlerts([FromServices] IGetMyPriceAlertsQuery query)
        {
            return Ok(_useCaseHandler.HandleQuery(query, 0));
        }

        [HttpPut("/api/price-alerts/{destinationId:int}")]
        public IActionResult SetPriceAlert(int destinationId, [FromBody] SetPriceAlertDto request, [FromServices] ISetPriceAlertCommand command)
        {
            request.DestinationId = destinationId;
            _useCaseHandler.HandleCommand(command, request);
            return NoContent();
        }

        [HttpDelete("/api/price-alerts/{destinationId:int}")]
        public IActionResult RemovePriceAlert(int destinationId, [FromServices] ISetPriceAlertCommand command)
        {
            _useCaseHandler.HandleCommand(command, new SetPriceAlertDto { DestinationId = destinationId, MaxPrice = null });
            return NoContent();
        }

        [HttpGet("/api/exchange-rates")]
        public IActionResult ExchangeRates([FromServices] IGetExchangeRatesQuery query)
        {
            return Ok(_useCaseHandler.HandleQuery(query, 0));
        }

        [HttpGet("/api/agencies/{id:int}")]
        public IActionResult AgencyPage(int id, [FromServices] IGetAgencyPageQuery query)
        {
            return Ok(_useCaseHandler.HandleQuery(query, id));
        }
    }
}
