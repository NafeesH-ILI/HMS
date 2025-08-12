using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace hms
{
    public class ErrorHandlerAttribute /*(ILogger<ErrorHandlerAttribute> logger)*/: ExceptionFilterAttribute
    {
        //private readonly ILogger<ErrorHandlerAttribute> _logger = logger;
        public override void OnException(ExceptionContext ctx)
        {
            ctx.Result = new BadRequestResult();
        }
    }
}
