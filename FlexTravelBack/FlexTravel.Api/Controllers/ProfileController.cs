using FlexTravel.Api.Core;
using FlexTravel.Application.Dto;
using FlexTravel.Application.UseCases;
using FlexTravel.Application.UseCases.Commands;
using FlexTravel.Application.UseCases.Queries;
using FlexTravel.Implementation;
using Microsoft.AspNetCore.Mvc;

namespace FlexTravel.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly UseCaseHandler _useCaseHandler;

        public ProfileController(UseCaseHandler handler)
        {
            _useCaseHandler = handler;
        }

        [HttpGet]
        public IActionResult Get([FromServices] IGetMyProfileQuery query)
        {
            return Ok(_useCaseHandler.HandleQuery(query, 0));
        }

        [HttpPut]
        public IActionResult Update([FromBody] UpdateProfileDto request, [FromServices] IUpdateMyProfileCommand command,
            [FromServices] JwtManager jwt, [FromServices] IUser user)
        {
            _useCaseHandler.HandleCommand(command, request);
            return Ok(new { token = jwt.MakeTokenFor(user.Id) });
        }

        [HttpPost("avatar")]
        [RequestSizeLimit(6 * 1024 * 1024)]
        public IActionResult UploadAvatar(IFormFile file, [FromServices] IChangeMyAvatarCommand command,
            [FromServices] JwtManager jwt, [FromServices] IUser user)
        {
            byte[] content = null;
            if (file != null)
            {
                using var stream = new MemoryStream();
                file.CopyTo(stream);
                content = stream.ToArray();
            }

            if (content == null || content.Length == 0)
            {
                return UnprocessableEntity(new { message = "Choose an image." });
            }

            var avatar = _useCaseHandler.HandleCommand(command, new UploadImageDto
            {
                FileName = file.FileName,
                ContentType = file.ContentType,
                Content = content
            });

            return Ok(new { avatar, token = jwt.MakeTokenFor(user.Id) });
        }

        [HttpDelete("avatar")]
        public IActionResult RemoveAvatar([FromServices] IChangeMyAvatarCommand command, [FromServices] JwtManager jwt,
            [FromServices] IUser user)
        {
            _useCaseHandler.HandleCommand(command, new UploadImageDto());
            return Ok(new { token = jwt.MakeTokenFor(user.Id) });
        }

        [HttpPost("password")]
        public IActionResult ChangePassword([FromBody] ChangePasswordDto request, [FromServices] IChangePasswordCommand command)
        {
            _useCaseHandler.HandleCommand(command, request);
            return NoContent();
        }
    }
}
