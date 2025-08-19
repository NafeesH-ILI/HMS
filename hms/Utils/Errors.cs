using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlTypes;
using System.Net.Http.Headers;

namespace hms.Utils
{
    public class ErrorHandlerAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext ctx)
        {
            Console.WriteLine(ctx.Exception);
            if (ctx.Exception is ErrNotFound)
            {
                ctx.Result = new NotFoundResult();
            }
            else if (ctx.Exception is ErrBadReq ||
                ctx.Exception is SqlTypeException ||
                ctx.Exception is DbUpdateException ||
                ctx.Exception is ErrBadPagination ||
                ctx.Exception is ArgumentException)
            {
                ctx.Result = new BadRequestResult();
            }
            else if (ctx.Exception is ErrUnauthorized)
            {
                ctx.Result = new UnauthorizedResult();
            }
            else
            {
                ctx.Result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }

    public class ErrNotFound() : Exception { }
    public class ErrBadReq() : Exception { }
    public class ErrBadPagination() : Exception { }
    public class ErrAlreadyExists() : Exception { }
    public class ErrUnauthorized() : Exception { }
}
