using Common.Utilities;
using Serilog.Context;

namespace RJOS.Middlewares;

public class UserLogMiddleware
{
    private readonly RequestDelegate _next;

    public UserLogMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public Task Invoke(HttpContext context)
    {
        if (context.Session.GetInt32("UserId") != null)
        {
            LogContext.PushProperty("SessionId", context.Session.Id);
            LogContext.PushProperty("UserId", context.Session.GetInt32("UserId"));
        }
        
        LogContext.PushProperty("IpAddress", ExtensionMethods.GetIpAddress());
        
        LogContext.PushProperty("MacAddress", ExtensionMethods.GetMacAddress());

        return _next(context);
    }
}