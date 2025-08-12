using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net.Http.Headers;

namespace hms
{
    public class ErrorHandlerAttribute /*(ILogger<ErrorHandlerAttribute> logger)*/: ExceptionFilterAttribute
    {
        //private readonly ILogger<ErrorHandlerAttribute> _logger = logger;
        public override void OnException(ExceptionContext ctx)
        {
            if (ctx.Exception is ErrNotFound)
            {
                ctx.Result = new NotFoundResult();
            }
            else if (ctx.Exception is ErrBadReq)
            {
                ctx.Result = new BadRequestResult();
            }
        }
    }

    public class ErrNotFound() : Exception { }

    public class ErrBadReq() : Exception { }
}
