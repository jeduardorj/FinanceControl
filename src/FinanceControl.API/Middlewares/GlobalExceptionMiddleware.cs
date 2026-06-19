using System.Net;
using System.Text.Json;
using FinanceControl.Domain.Exceptions;

namespace FinanceControl.API.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger)
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exceção não tratada: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse();

        switch (exception)
        {
            case ValidationException validationEx:
                context.Response.StatusCode = validationEx.StatusCode;
                response.Status = validationEx.StatusCode;
                response.Title = "Erro de Validação";
                response.Detail = validationEx.Message;
                response.Errors = validationEx.Errors;
                break;

            case AppException appEx:
                context.Response.StatusCode = appEx.StatusCode;
                response.Status = appEx.StatusCode;
                response.Title = GetTitleForStatusCode(appEx.StatusCode);
                response.Detail = appEx.Message;
                break;

            default:
                context.Response.StatusCode =
                    (int)HttpStatusCode.InternalServerError;
                response.Status = (int)HttpStatusCode.InternalServerError;
                response.Title = "Erro Interno do Servidor";
                response.Detail = "Ocorreu um erro inesperado. Tente novamente.";
                break;
        }

        response.Instance = context.Request.Path;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(response, options);
        await context.Response.WriteAsync(json);
    }

    private static string GetTitleForStatusCode(int statusCode) =>
        statusCode switch
        {
            400 => "Requisição Inválida",
            401 => "Não Autorizado",
            403 => "Acesso Negado",
            404 => "Não Encontrado",
            409 => "Conflito",
            422 => "Entidade Não Processável",
            _ => "Erro"
        };
}

public class ErrorResponse
{
    public int Status { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Detail { get; set; } = string.Empty;
    public string Instance { get; set; } = string.Empty;
    public IEnumerable<string>? Errors { get; set; }
}