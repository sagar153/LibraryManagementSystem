namespace LibraryManagementSystem.Middlewares;

using System.Net;

/// <summary>
/// Middleware for global exception handling and error responses
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            _logger.LogError($"Unhandled exception: {ex.Message}\nStackTrace: {ex.StackTrace}");
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// Handle exceptions and return appropriate HTTP response
    /// </summary>
    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse
        {
            Success = false,
            Message = "An error occurred while processing the request",
            StatusCode = (int)HttpStatusCode.InternalServerError
        };

        switch (exception)
        {
            case ArgumentNullException argNullEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Message = $"Required parameter is null: {argNullEx.ParamName}";
                break;

            case ArgumentException argEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Message = $"Invalid argument: {argEx.Message}";
                break;

            case InvalidOperationException invalidOpEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Message = $"Invalid operation: {invalidOpEx.Message}";
                break;

            case KeyNotFoundException keyNotFoundEx:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                response.Message = $"Resource not found: {keyNotFoundEx.Message}";
                break;

            case UnauthorizedAccessException unauthorizedEx:
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response.Message = $"Unauthorized access: {unauthorizedEx.Message}";
                break;

            case TimeoutException timeoutEx:
                response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                response.Message = $"Request timeout: {timeoutEx.Message}";
                break;

            case HttpRequestException httpEx:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Message = $"HTTP request error: {httpEx.Message}";
                break;

            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Message = "An unexpected error occurred. Please try again later.";
                break;
        }

        context.Response.StatusCode = response.StatusCode;
        return context.Response.WriteAsJsonAsync(response);
    }
}

/// <summary>
/// Error response model
/// </summary>
public class ErrorResponse
{
    public bool Success { get; set; } = false;
    public string Message { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public Dictionary<string, object>? Details { get; set; }
    public string? TraceId { get; set; }
}
