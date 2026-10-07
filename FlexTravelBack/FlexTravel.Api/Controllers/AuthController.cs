using FlexTravel.Api.Core;
using FlexTravel.Application.Dto;
using FlexTravel.Application.UseCases.Commands;
using FlexTravel.Implementation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FlexTravel.Api.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JwtManager _jwtManager;
        private readonly UseCaseHandler _useCaseHandler;

        public AuthController(JwtManager jwtManager, UseCaseHandler useCaseHandler)
        {
            _jwtManager = jwtManager;
            _useCaseHandler = useCaseHandler;
        }

        [HttpPost("/api/register")]
        public IActionResult Register([FromBody] RegisterUserDto request, [FromServices] IRegisterUserCommand command)
        {
            _useCaseHandler.HandleCommand(command, request);
            var pair = _jwtManager.SignIn(request.Email.Trim(), request.Password, rememberMe: false);
            return StatusCode(StatusCodes.Status201Created, pair);
        }

        [HttpPost("/api/token")]
        [AllowAnonymous]
        [EnableRateLimiting(SignInRateLimit)]
        public IActionResult Token([FromBody] TokenRequestDto request)
        {
            try
            {
                return Ok(_jwtManager.SignIn(request.Email, request.Password, request.RememberMe));
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Wrong e-mail or password." });
            }
            catch (AccountLockedException ex)
            {
                return StatusCode(StatusCodes.Status429TooManyRequests, new { message = ex.Message });
            }
        }

        [HttpPost("/api/token/refresh")]
        [AllowAnonymous]
        public IActionResult Refresh([FromBody] RefreshTokenDto request)
        {
            var pair = _jwtManager.Refresh(request?.RefreshToken);
            return pair == null
                ? Unauthorized(new { message = "Your session has ended. Please sign in again." })
                : Ok(pair);
        }

        [HttpPost("/api/token/revoke")]
        [AllowAnonymous]
        public IActionResult Revoke([FromBody] RefreshTokenDto request)
        {
            _jwtManager.SignOut(request?.RefreshToken);
            return NoContent();
        }

        public const string SignInRateLimit = "sign-in";
    }
}
