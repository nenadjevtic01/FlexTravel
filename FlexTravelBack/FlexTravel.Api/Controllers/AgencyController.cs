using FlexTravel.Application.Dto;
using FlexTravel.Application.UseCases.Commands;
using FlexTravel.Application.UseCases.Queries;
using FlexTravel.Implementation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FlexTravel.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgencyController : ControllerBase
    {
        private readonly UseCaseHandler _useCaseHandler;

        public AgencyController(UseCaseHandler handler)
        {
            _useCaseHandler = handler;
        }

        [HttpGet("trips")]
        public IActionResult Trips([FromServices] IGetAgencyTripsQuery query)
        {
            return Ok(_useCaseHandler.HandleQuery(query, 0));
        }

        [HttpGet("trip-options")]
        public IActionResult TripOptions([FromServices] IGetTripFormOptionsQuery query)
        {
            return Ok(_useCaseHandler.HandleQuery(query, 0));
        }

        [HttpPost("trips")]
        public IActionResult CreateTrip([FromBody] CreateTripDto request, [FromServices] ICreateTripCommand command)
        {
            var id = _useCaseHandler.HandleCommand(command, request);
            return StatusCode(StatusCodes.Status201Created, new { id });
        }

        [HttpGet("trips/{id:int}")]
        public IActionResult GetTrip(int id, [FromServices] IGetTripForEditQuery query)
        {
            return Ok(_useCaseHandler.HandleQuery(query, id));
        }

        [HttpPut("trips/{id:int}")]
        public IActionResult UpdateTrip(int id, [FromBody] UpdateTripDto request, [FromServices] IUpdateTripCommand command)
        {
            request.Id = id;
            _useCaseHandler.HandleCommand(command, request);
            return NoContent();
        }

        [HttpGet("trips/{id:int}/manifest")]
        public IActionResult Manifest(int id, [FromServices] IGetTripManifestQuery query)
        {
            return Ok(_useCaseHandler.HandleQuery(query, id));
        }

        [HttpGet("calendar")]
        public IActionResult Calendar([FromQuery] AgencyCalendarRequestDto request, [FromServices] IGetAgencyCalendarQuery query)
        {
            return Ok(_useCaseHandler.HandleQuery(query, request));
        }

        [HttpGet("report")]
        public IActionResult Report([FromQuery] int year, [FromServices] IGetAgencyReportQuery query)
        {
            return Ok(_useCaseHandler.HandleQuery(query, year));
        }

        [HttpPut("profile")]
        public IActionResult UpdateProfile([FromBody] UpdateAgencyProfileDto request, [FromServices] IUpdateAgencyProfileCommand command)
        {
            _useCaseHandler.HandleCommand(command, request);
            return NoContent();
        }

        [HttpPost("logo")]
        [RequestSizeLimit(6 * 1024 * 1024)]
        public IActionResult UploadLogo(IFormFile file, [FromServices] IUploadImageCommand command)
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

        [HttpPost("trips/{id:int}/departures")]
        public IActionResult AddDeparture(int id, [FromBody] AddDepartureDto request, [FromServices] IAddDepartureCommand command)
        {
            request.TripId = id;
            var tripId = _useCaseHandler.HandleCommand(command, request);
            return StatusCode(StatusCodes.Status201Created, new { id = tripId });
        }

        [HttpPost("trips/{id}/deactivate")]
        public IActionResult DeactivateTrip(int id, [FromServices] IDeactivateTripCommand command)
        {
            _useCaseHandler.HandleCommand(command, id);
            return NoContent();
        }

        [HttpGet("reservations")]
        public IActionResult Reservations([FromQuery] ReservationSearchDto search, [FromServices] IGetAgencyReservationsQuery query)
        {
            return Ok(_useCaseHandler.HandleQuery(query, search));
        }

        [HttpPost("reservations/{id}/confirm")]
        public IActionResult Confirm(int id, [FromServices] IDecideReservationCommand command)
        {
            _useCaseHandler.HandleCommand(command, new DecideReservationDto { ReservationId = id, Confirm = true });
            return NoContent();
        }

        [HttpPost("reservations/{id}/reject")]
        public IActionResult Reject(int id, [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] RejectReservationRequest body, [FromServices] IDecideReservationCommand command)
        {
            _useCaseHandler.HandleCommand(command, new DecideReservationDto { ReservationId = id, Confirm = false, Reason = body?.Reason });
            return NoContent();
        }

        [HttpGet("promo-codes")]
        public IActionResult PromoCodes([FromServices] IGetPromoCodesQuery query)
        {
            return Ok(_useCaseHandler.HandleQuery(query, 0));
        }

        [HttpPost("promo-codes")]
        public IActionResult CreatePromoCode([FromBody] CreatePromoCodeDto request, [FromServices] ICreatePromoCodeCommand command)
        {
            var id = _useCaseHandler.HandleCommand(command, request);
            return StatusCode(StatusCodes.Status201Created, new { id });
        }

        [HttpPost("promo-codes/{id:int}/activate")]
        public IActionResult ActivatePromoCode(int id, [FromServices] ISetPromoCodeActiveCommand command)
        {
            _useCaseHandler.HandleCommand(command, new SetPromoCodeActiveDto { Id = id, IsActive = true });
            return NoContent();
        }

        [HttpPost("promo-codes/{id:int}/deactivate")]
        public IActionResult DeactivatePromoCode(int id, [FromServices] ISetPromoCodeActiveCommand command)
        {
            _useCaseHandler.HandleCommand(command, new SetPromoCodeActiveDto { Id = id, IsActive = false });
            return NoContent();
        }

        public class RejectReservationRequest
        {
            public string Reason { get; set; }
        }
    }
}
