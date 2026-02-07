namespace LibraryManagementSystem.Middlewares;

/// <summary>
/// Extension methods for middleware registration
/// </summary>
public static class MiddlewareExtensions
{
    /// <summary>
    /// Add exception handling middleware
    /// </summary>
    public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }

    /// <summary>
    /// Add authentication middleware
    /// </summary>
    public static IApplicationBuilder UseCustomAuthentication(this IApplicationBuilder app)
    {
        return app.UseMiddleware<AuthenticationMiddleware>();
    }

    /// <summary>
    /// Add authorization middleware
    /// </summary>
    public static IApplicationBuilder UseCustomAuthorization(this IApplicationBuilder app)
    {
        return app.UseMiddleware<AuthorizationMiddleware>();
    }

    /// <summary>
    /// Add all custom middlewares in the correct order
    /// </summary>
    public static IApplicationBuilder UseCustomMiddlewares(this IApplicationBuilder app)
    {
        // Exception handling should be first to catch all exceptions
        app.UseGlobalExceptionHandling();
        
        // Then authentication to extract tokens
        app.UseCustomAuthentication();
        
        // Then authorization to verify access
        app.UseCustomAuthorization();

        return app;
    }
}
