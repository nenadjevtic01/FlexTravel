using FlexTravel.Application.Dto;
using FlexTravel.Application.UseCases.Commands;
using FlexTravel.Application.UseCases.Queries;
using FlexTravel.Implementation;
using Microsoft.AspNetCore.Mvc;

namespace FlexTravel.Api.Controllers
{
    [Route("api/admin/catalog")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly UseCaseHandler _useCaseHandler;

        public CatalogController(UseCaseHandler handler)
        {
            _useCaseHandler = handler;
        }

        [HttpGet("options")]
        public IActionResult Options([FromServices] IGetCatalogOptionsQuery query)
        {
            return Ok(_useCaseHandler.HandleQuery(query, 0));
        }

        [HttpPost("countries")]
        public IActionResult CreateCountry([FromBody] SaveCountryDto request, [FromServices] ISaveCountryCommand command)
        {
            request.Id = 0;
            return StatusCode(StatusCodes.Status201Created, new { id = _useCaseHandler.HandleCommand(command, request) });
        }

        [HttpPut("countries/{id:int}")]
        public IActionResult UpdateCountry(int id, [FromBody] SaveCountryDto request, [FromServices] ISaveCountryCommand command)
        {
            request.Id = id;
            _useCaseHandler.HandleCommand(command, request);
            return NoContent();
        }

        [HttpGet("destinations/{id:int}")]
        public IActionResult GetDestination(int id, [FromServices] IGetDestinationForEditQuery query)
        {
            return Ok(_useCaseHandler.HandleQuery(query, id));
        }

        [HttpPost("destinations")]
        public IActionResult CreateDestination([FromBody] SaveDestinationDto request, [FromServices] ISaveDestinationCommand command)
        {
            request.Id = 0;
            return StatusCode(StatusCodes.Status201Created, new { id = _useCaseHandler.HandleCommand(command, request) });
        }

        [HttpPut("destinations/{id:int}")]
        public IActionResult UpdateDestination(int id, [FromBody] SaveDestinationDto request, [FromServices] ISaveDestinationCommand command)
        {
            request.Id = id;
            _useCaseHandler.HandleCommand(command, request);
            return NoContent();
        }

        [HttpPost("destinations/{id:int}/{mode:regex(^(hide|show)$)}")]
        public IActionResult SetDestinationActive(int id, string mode, [FromServices] ISetDestinationActiveCommand command)
        {
            _useCaseHandler.HandleCommand(command, new SetActiveDto { Id = id, IsActive = mode == "show" });
            return NoContent();
        }

        [HttpPost("hotels/{id:int}/{mode:regex(^(hide|show)$)}")]
        public IActionResult SetHotelActive(int id, string mode, [FromServices] ISetHotelActiveCommand command)
        {
            _useCaseHandler.HandleCommand(command, new SetActiveDto { Id = id, IsActive = mode == "show" });
            return NoContent();
        }

        [HttpPost("images/cleanup")]
        public IActionResult CleanUpImages([FromServices] ICleanUpImagesCommand command)
        {
            return Ok(new { deleted = _useCaseHandler.HandleCommand(command, 0) });
        }

        [HttpPost("destinations/{destinationId:int}/hotels")]
        public IActionResult CreateHotel(int destinationId, [FromBody] SaveHotelDto request, [FromServices] ISaveHotelCommand command)
        {
            request.Id = 0;
            request.DestinationId = destinationId;
            return StatusCode(StatusCodes.Status201Created, new { id = _useCaseHandler.HandleCommand(command, request) });
        }

        [HttpPut("hotels/{id:int}")]
        public IActionResult UpdateHotel(int id, [FromBody] SaveHotelDto request, [FromServices] ISaveHotelCommand command)
        {
            request.Id = id;
            _useCaseHandler.HandleCommand(command, request);
            return NoContent();
        }

        [HttpPost("destinations/{destinationId:int}/sights")]
        public IActionResult CreateSight(int destinationId, [FromBody] SaveSightDto request, [FromServices] ISaveSightCommand command)
        {
            request.Id = 0;
            request.DestinationId = destinationId;
            return StatusCode(StatusCodes.Status201Created, new { id = _useCaseHandler.HandleCommand(command, request) });
        }

        [HttpPut("sights/{id:int}")]
        public IActionResult UpdateSight(int id, [FromBody] SaveSightDto request, [FromServices] ISaveSightCommand command)
        {
            request.Id = id;
            _useCaseHandler.HandleCommand(command, request);
            return NoContent();
        }

        [HttpDelete("sights/{id:int}")]
        public IActionResult DeleteSight(int id, [FromServices] IDeleteSightCommand command)
        {
            _useCaseHandler.HandleCommand(command, id);
            return NoContent();
        }

        [HttpPost("images")]
        [RequestSizeLimit(6 * 1024 * 1024)]
        public IActionResult UploadImage(IFormFile file, [FromServices] IUploadImageCommand command)
        {
            byte[] content = null;
            if (file != null)
            {
                using var stream = new MemoryStream();
                file.CopyTo(stream);
                content = stream.ToArray();
            }

            var src = _useCaseHandler.HandleCommand(command, new UploadImageDto
            {
                FileName = file?.FileName,
                ContentType = file?.ContentType,
                Content = content
            });

            return StatusCode(StatusCodes.Status201Created, new { src });
        }
    }
}
