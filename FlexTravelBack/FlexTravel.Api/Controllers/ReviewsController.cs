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
    public class ReviewsController : ControllerBase
    {
        private readonly UseCaseHandler _useCaseHandler;

        public ReviewsController(UseCaseHandler handler)
        {
            _useCaseHandler = handler;
        }

        [HttpGet]
        public IActionResult Get([FromQuery] ReviewSearchDto search, [FromServices] IGetReviewsQuery query)
        {
            return Ok(_useCaseHandler.HandleQuery(query, search));
        }

        [HttpPost]
        public IActionResult Post([FromBody] CreateReviewDto request, [FromServices] ICreateReviewCommand command)
        {
            var id = _useCaseHandler.HandleCommand(command, request);
            return StatusCode(StatusCodes.Status201Created, new { id });
        }

        [HttpPost("{id:int}/reply")]
        public IActionResult Reply(int id, [FromBody] ReplyToReviewDto request, [FromServices] IReplyToReviewCommand command)
        {
            request.ReviewId = id;
            _useCaseHandler.HandleCommand(command, request);
            return NoContent();
        }

        [HttpPost("{id:int}/hide")]
        public IActionResult Hide(int id, [FromBody] ModerateReviewDto request, [FromServices] IModerateReviewCommand command)
        {
            request.ReviewId = id;
            request.Visible = false;
            _useCaseHandler.HandleCommand(command, request);
            return NoContent();
        }

        [HttpPost("{id:int}/show")]
        public IActionResult Show(int id, [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] ModerateReviewDto request,
            [FromServices] IModerateReviewCommand command)
        {
            request ??= new ModerateReviewDto();
            request.ReviewId = id;
            request.Visible = true;
            _useCaseHandler.HandleCommand(command, request);
            return NoContent();
        }
    }
}
