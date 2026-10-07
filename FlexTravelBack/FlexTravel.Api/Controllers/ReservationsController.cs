using FlexTravel.Application.Dto;
using FlexTravel.Application.UseCases.Commands;
using FlexTravel.Application.UseCases.Queries;
using FlexTravel.Implementation;
using Microsoft.AspNetCore.Mvc;

namespace FlexTravel.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly UseCaseHandler _useCaseHandler;

        public ReservationsController(UseCaseHandler handler)
        {
            _useCaseHandler = handler;
        }

        [HttpGet("mine")]
        public IActionResult Mine([FromQuery] ReservationSearchDto search, [FromServices] IGetMyReservationsQuery query)
        {
            return Ok(_useCaseHandler.HandleQuery(query, search));
        }

        [HttpGet("{id:int}")]
        public IActionResult Get(int id, [FromServices] IGetReservationQuery query)
        {
            return Ok(_useCaseHandler.HandleQuery(query, id));
        }

        [HttpPost]
        public IActionResult Post([FromBody] BookingRequestDto request, [FromServices] ICreateReservationCommand command)
        {
            var created = _useCaseHandler.HandleCommand(command, request);
            return StatusCode(StatusCodes.Status201Created, created);
        }

        [HttpPut("{id:int}/travelers")]
        public IActionResult Travelers(int id, [FromBody] List<TravelerDto> travelers, [FromServices] IUpdateTravelersCommand command)
        {
            _useCaseHandler.HandleCommand(command, new UpdateTravelersDto { ReservationId = id, Travelers = travelers ?? new List<TravelerDto>() });
            return NoContent();
        }

        [HttpGet("{id:int}/messages")]
        public IActionResult Messages(int id, [FromServices] IGetReservationMessagesQuery query)
        {
            return Ok(_useCaseHandler.HandleQuery(query, id));
        }

        [HttpPost("{id:int}/messages")]
        public IActionResult SendMessage(int id, [FromBody] SendMessageDto request, [FromServices] ISendReservationMessageCommand command)
        {
            request.ReservationId = id;
            var messageId = _useCaseHandler.HandleCommand(command, request);
            return StatusCode(StatusCodes.Status201Created, new { id = messageId });
        }

        [HttpPost("{id:int}/payments")]
        public IActionResult RecordPayment(int id, [FromBody] RecordPaymentDto request, [FromServices] IRecordPaymentCommand command)
        {
            request.ReservationId = id;
            var paymentId = _useCaseHandler.HandleCommand(command, request);
            return StatusCode(StatusCodes.Status201Created, new { id = paymentId });
        }

        [HttpPost("{id}/cancel")]
        public IActionResult Cancel(int id, [FromServices] ICancelReservationCommand command)
        {
            _useCaseHandler.HandleCommand(command, id);
            return NoContent();
        }
    }
}
