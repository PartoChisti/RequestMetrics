using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using System.Diagnostics;

namespace TestWebApp.Attributes
{
    public class LogExecutionTimeAttribute : ActionFilterAttribute
    {
        private Stopwatch _sw;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            _sw = Stopwatch.StartNew();
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            _sw.Stop();

            Log.ForContext("Controller", context.Controller.ToString())
               .ForContext("Action", context.ActionDescriptor.DisplayName)
               .ForContext("DurationMs", _sw.ElapsedMilliseconds)
               .Information("ACTION_METRIC");
        }
    }

}
