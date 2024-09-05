using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Backend.Application.Middlewares
{
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
            catch (Exception error)
            {
                logger.LogError(0, error, "An unexpected exception occurred during request handling.");

                var response = context.Response;
                response.ContentType = "application/json";

                response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var result = JsonSerializer.Serialize(new { message = error?.Message });
                await response.WriteAsync(result);
            }
        }
    }
}
