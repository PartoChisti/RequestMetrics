using Microsoft.AspNetCore.SignalR;
using Serilog.Context;

public class SerilogHubFilter : IHubFilter
{
    public async ValueTask<object> InvokeMethodAsync(
        HubInvocationContext context,
        Func<HubInvocationContext, ValueTask<object>> next)
    {
        LogContext.PushProperty("ConnectionId", context.Context.ConnectionId);
        return await next(context);
    }
}
