using Elo.Application.Common;
using FluentValidation;
using System.Net;
using System.Text.Json;

namespace Elo.Api.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleAsync(context, ex, logger);
        }
    }

    private static Task HandleAsync(HttpContext context, Exception ex, ILogger logger)
    {
        logger.LogError(ex, "Erro não tratado: {Message}", ex.Message);

        var (status, message) = ex switch
        {
            NotFoundException nf => (HttpStatusCode.NotFound, nf.Message),
            ConflictException cf => (HttpStatusCode.Conflict, cf.Message),
            ValidationAppException ve => (HttpStatusCode.BadRequest, ve.Message),
            ValidationException fv => (HttpStatusCode.BadRequest,
                string.Join(" ", fv.Errors.Select(e => e.ErrorMessage))),
            _ => (HttpStatusCode.InternalServerError, "Erro interno do servidor."),
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)status;

        var body = JsonSerializer.Serialize(new { erro = message });
        return context.Response.WriteAsync(body);
    }
}
