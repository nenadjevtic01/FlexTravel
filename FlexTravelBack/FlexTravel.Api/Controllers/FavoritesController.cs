using FlexTravel.Application.Dto;
using FlexTravel.Application.UseCases.Commands;
using FlexTravel.Application.UseCases.Queries;
using FlexTravel.Implementation;
using Microsoft.AspNetCore.Mvc;

namespace FlexTravel.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoritesController : ControllerBase
    {
        private readonly UseCaseHandler _useCaseHandler;

        public FavoritesController(UseCaseHandler handler)
        {
            _useCaseHandler = handler;
        }

        [HttpGet]
        public IActionResult Get([FromServices] IGetMyFavoritesQuery query)
        {
            return Ok(_useCaseHandler.HandleQuery(query, 0));
        }

        [HttpPut("{destinationId:int}")]
        public IActionResult Add(int destinationId, [FromServices] ISetFavoriteCommand command)
        {
            _useCaseHandler.HandleCommand(command, new SetFavoriteDto { DestinationId = destinationId, IsFavorite = true });
            return NoContent();
        }

        [HttpDelete("{destinationId:int}")]
        public IActionResult Remove(int destinationId, [FromServices] ISetFavoriteCommand command)
        {
            _useCaseHandler.HandleCommand(command, new SetFavoriteDto { DestinationId = destinationId, IsFavorite = false });
            return NoContent();
        }
    }
}
