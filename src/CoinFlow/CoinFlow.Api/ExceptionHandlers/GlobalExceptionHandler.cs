using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using CoinFlow.Domain.Exceptions.Users;
using CoinFlow.Application.Users.Exceptions;
using CoinFlow.Domain.Exceptions;

namespace CoinFlow.Api.ExceptionHandlers;

internal sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) => _logger = logger;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken ct)
    {
        var problem = MapToProblemDetails(exception);

        if (problem.Status >= 500)
            _logger.LogError(exception, "Exceção não tratada: {Message}", exception.Message);
        else
            _logger.LogWarning("Exceção tratada: {Type} - {Message}", 
                exception.GetType(), exception.Message);

        httpContext.Response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problem, problem.GetType(), ct);
        return true;
    }

    private static ProblemDetails MapToProblemDetails(Exception exception) => exception switch
    {
        ValidationException validation => new ValidationProblemDetails(
            validation.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(x => x.Key, x => x.Select(x => x.ErrorMessage).ToArray()))
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Erro de validação.",
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
        },

        InvalidEmailException => new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Formato de email inválido.",
            Detail = exception.Message,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
        },

        EmailAlreadyExistsException => new ProblemDetails
        {
            Status = StatusCodes.Status409Conflict,
            Title = "Email já existe.",
            Detail = exception.Message,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8"
        },

        InvalidCredentialsException => new ProblemDetails
        {
            Status = StatusCodes.Status401Unauthorized,
            Title = "Credenciais inválidas.",
            Detail = exception.Message,
            Type = "https://tools.ietf.org/html/rfc7235#section-3.1"
        },

        InvalidRefreshTokenException => new ProblemDetails
        {
            Status = StatusCodes.Status401Unauthorized,
            Title = "Token de atualização inválido.",
            Detail = exception.Message,
            Type = "https://tools.ietf.org/html/rfc7235#section-3.1"
        },

        DomainExceptionBase domain => new ProblemDetails
        {
            Status = StatusCodes.Status422UnprocessableEntity,
            Title = "Violação de regra de negócio.",
            Detail = domain.Message,
            Type = "https://tools.ietf.org/html/rfc4918#section-11.2"
        },

        _ => new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Erro interno do servidor.",
            Detail = "Um erro inesperado ocorreu."
        }
    };
}
