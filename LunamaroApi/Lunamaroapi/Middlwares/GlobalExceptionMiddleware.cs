using System.Net;
using System.Text.Json;

namespace Lunamaroapi.Middlwares
{
    public class GlobalExceptionMiddleware : IMiddleware
    {
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(ILogger<GlobalExceptionMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                // Logging
                _logger.LogError(ex, "Unhandled exception on path {Path}", context.Request.Path);

                // Response
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var responseBody = new
                {
                    message = "An internal server error occurred. Please try again later.",
                    traceId = context.TraceIdentifier
                };

                var json = JsonSerializer.Serialize(responseBody);
                await context.Response.WriteAsync(json);
            }
        }
    }
}
