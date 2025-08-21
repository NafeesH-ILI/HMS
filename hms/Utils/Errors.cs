using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlTypes;
using System.Text.Json;

namespace hms.Utils
{
    public class CustomExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<CustomExceptionHandlerMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<CustomExceptionHandlerMiddleware> _logger = logger;

        private void ExceptionHandle(HttpContext ctx, Exception ex)
        {
            _logger.LogWarning(ex, "handling exception");
            string message = "";
            int code;
            if (ex is CustomException)
            {
                message = ((CustomException)ex).CustomMessage ?? message;
                code = ((CustomException)ex).Code;
            }
            else if (ex is SqlTypeException ||
                ex is DbUpdateException ||
                ex is AutoMapperMappingException ||
                ex is ArgumentException)
            {
                message = "Invalid Data or Schema";
                code = StatusCodes.Status400BadRequest;
            }
            else
            {
                _logger.LogError(ex, "Unhandled Exception Type");
                message = "Invalid Data or Schema";
                code = StatusCodes.Status400BadRequest;
            }
            ctx.Response.ContentType = "application/json";
            ctx.Response.StatusCode = code;
            ctx.Response.WriteAsync(JsonSerializer.Serialize(new {Success = false, Message = message}));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                ExceptionHandle(context, ex);
            }
        }
    }

    public static class CustomExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomExceptionHandlerMiddleware>();
        }
    }

    public class CustomException(int code, string? msg = null) : Exception
    {
        public readonly string? CustomMessage = msg;
        public readonly int Code = code;
    }

    public class ErrNotFound(string? msg = null) : CustomException(StatusCodes.Status404NotFound,
        msg ?? "Not Found") { }
    public class ErrBadReq(string? msg = null) : CustomException(StatusCodes.Status400BadRequest,
        msg ?? "Bad Request") { }
    public class ErrBadPagination(string? msg = null) : CustomException(StatusCodes.Status400BadRequest,
        msg ?? "Bad Pagination") { }
    public class ErrAlreadyExists(string? msg = null) : CustomException(StatusCodes.Status409Conflict,
        msg ?? "Already Exists") { }
    public class ErrUnauthorized(string? msg = null) : CustomException(StatusCodes.Status401Unauthorized,
        msg ?? "Unauthorized") { }
    public class ErrForbidden(string? msg = null) : CustomException(StatusCodes.Status403Forbidden,
        msg ?? "Forbidden") { }
    public class ErrNotLegal(string? msg = null) : CustomException(
        StatusCodes.Status451UnavailableForLegalReasons, msg ?? "Not Available for Legal Reason") { }
}
