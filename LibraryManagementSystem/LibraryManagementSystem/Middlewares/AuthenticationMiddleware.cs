namespace LibraryManagementSystem.Middlewares;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

/// <summary>
/// Middleware for JWT token validation and extraction
/// </summary>
public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuthenticationMiddleware> _logger;

    public AuthenticationMiddleware(RequestDelegate next, ILogger<AuthenticationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Extract and validate JWT token from Authorization header
            var token = ExtractTokenFromHeader(context);

            if (!string.IsNullOrEmpty(token))
            {
                // Add token to context items for later use
                context.Items["Token"] = token;
                
                // Extract claims from token (without validation - validation done by JWT Bearer middleware)
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadToken(token) as JwtSecurityToken;
                    
                    if (jwtToken != null)
                    {
                        context.Items["JwtToken"] = jwtToken;
                        _logger.LogInformation($"Token extracted and available in context. User: {jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Failed to read JWT token: {ex.Message}");
                    // Continue anyway - JWT Bearer middleware will validate
                }
            }
            else
            {
                _logger.LogDebug("No authorization token found in request");
            }

            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in authentication middleware: {ex.Message}");
            // Continue to next middleware - let exception handling middleware handle it
            await _next(context);
        }
    }

    /// <summary>
    /// Extract JWT token from Authorization header
    /// </summary>
    private static string? ExtractTokenFromHeader(HttpContext context)
    {
        const string authorizationHeader = "Authorization";
        const string bearer = "Bearer ";

        if (!context.Request.Headers.TryGetValue(authorizationHeader, out var headerValue))
            return null;

        var authHeader = headerValue.ToString();
        
        if (!authHeader.StartsWith(bearer, StringComparison.OrdinalIgnoreCase))
            return null;

        return authHeader[bearer.Length..];
    }
}
