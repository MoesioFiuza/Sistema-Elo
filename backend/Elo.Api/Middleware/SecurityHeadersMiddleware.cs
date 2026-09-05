namespace Elo.Api.Middleware;

public class SecurityHeadersMiddleware(RequestDelegate next)
{
    public Task InvokeAsync(HttpContext context)
    {
        var headers = context.Response.Headers;
        headers["X-Content-Type-Options"] = "nosniff";
        headers["X-Frame-Options"] = "DENY";
        headers["Referrer-Policy"] = "same-origin";
        headers["X-Permitted-Cross-Domain-Policies"] = "none";
        headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=()";
        return next(context);
    }
}
