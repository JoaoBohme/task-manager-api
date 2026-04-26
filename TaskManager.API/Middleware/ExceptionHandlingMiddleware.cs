using FluentValidation;
using TaskManager.API.Models;
using TaskManager.Application.Common.Exceptions;

namespace TaskManager.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "Erro nao tratado durante a requisicao {TraceId}", context.TraceIdentifier);

        var (statusCode, message, details) = exception switch
        {
            ValidationException validationException => (
                StatusCodes.Status400BadRequest,
                "Erro de validacao.",
                validationException.Errors.Select(error => new
                {
                    error.PropertyName,
                    error.ErrorMessage
                })),
            NotFoundException notFoundException => (
                StatusCodes.Status404NotFound,
                notFoundException.Message,
                null),
            ConflictException conflictException => (
                StatusCodes.Status409Conflict,
                conflictException.Message,
                null),
            UnauthorizedException unauthorizedException => (
                StatusCodes.Status401Unauthorized,
                unauthorizedException.Message,
                null),
            _ => (
                StatusCodes.Status500InternalServerError,
                "Ocorreu um erro interno ao processar a requisicao.",
                null)
        };

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse
        {
            TraceId = context.TraceIdentifier,
            Message = message,
            Details = details,
            StatusCode = statusCode
        };

        await context.Response.WriteAsJsonAsync(response);
    }
}
