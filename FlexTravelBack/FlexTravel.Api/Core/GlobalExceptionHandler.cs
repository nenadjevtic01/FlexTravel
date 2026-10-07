using FlexTravel.Application.Exceptions;
using FlexTravel.Application.Loggers;
using FlexTravel.Implementation.Exceptions;
using FluentValidation;

namespace FlexTravel.Api.Core
{
    public class GlobalExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly IExceptionLogger _logger;

        public GlobalExceptionHandler(RequestDelegate next, IExceptionLogger logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (ValidationException ex)
            {
                httpContext.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                await httpContext.Response.WriteAsJsonAsync(new
                {
                    message = ex.Errors.FirstOrDefault()?.ErrorMessage ?? "The request is not valid.",
                    errors = ex.Errors.Select(e => new { property = e.PropertyName, error = e.ErrorMessage })
                });
            }
            catch (Exception ex)
            {
                _logger.Log(ex);

                if (httpContext.Response.HasStarted)
                {
                    throw;
                }

                var (statusCode, message) = ex switch
                {
                    ForbiddenUseCaseExecutionException => (StatusCodes.Status403Forbidden, ex.Message),
                    ConflictException => (StatusCodes.Status409Conflict, ex.Message),
                    EntityNotFoundException => (StatusCodes.Status404NotFound, ex.Message),
                    _ => (StatusCodes.Status500InternalServerError, "There was an error, please contact support.")
                };

                httpContext.Response.StatusCode = statusCode;
                await httpContext.Response.WriteAsJsonAsync(new { message });
            }
        }
    }
}
