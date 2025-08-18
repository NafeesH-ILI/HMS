using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data.SqlTypes;
using System.Net.Http.Headers;

namespace hms.Common
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
                ctx.Exception is ErrBadPagination)
            {
                ctx.Result = new BadRequestResult();
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
}
