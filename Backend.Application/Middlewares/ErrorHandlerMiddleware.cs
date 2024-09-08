using System.Net;
using Backend.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Backend.Application.Middlewares
{
    public record ExceptionResponse(HttpStatusCode StatusCode, string Description);

    /// <summary>
    /// Represents the global error middleware class.
    /// </summary>
    /// <param name="next">The next middleware.</param>
    public sealed class ErrorHandlerMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate next = next;

        /// <inheritdoc/>
        public async Task Invoke(HttpContext context, ILogger<ErrorHandlerMiddleware> logger)
        {
            try
            {
                await next(context);
            }
            catch (Exception exception)
            {
                logger.LogError(0, exception, "An unexpected exception occurred during request handling.");

                ExceptionResponse response = exception switch
                {
                    ForbiddenException _ => new ExceptionResponse(HttpStatusCode.Unauthorized, exception.Message),
                    AppException => new ExceptionResponse(HttpStatusCode.BadRequest, exception.Message),
                    _ => new ExceptionResponse(HttpStatusCode.InternalServerError, exception.Message)
                };

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)response.StatusCode;
                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}