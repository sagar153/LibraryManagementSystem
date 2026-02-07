namespace LibraryManagementSystem.Middlewares;

using System.Security.Claims;
using LibraryManagementSystem.Services.Interfaces;

/// <summary>
/// Middleware for authorization and role-based access control
/// </summary>
public class AuthorizationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuthorizationMiddleware> _logger;

    // Routes that don't require authorization
    private static readonly HashSet<string> PublicRoutes = new(StringComparer.OrdinalIgnoreCase)
    {
        "/api/v1/auth/register",
        "/api/v1/auth/login",
        "/api/v1/auth/refresh",
        "/health",
        "/health/ready",
        "/health/live"
    };

    // Routes that require Librarian or Admin role
    private static readonly HashSet<string> LibrarianOnlyPrefixes = new(StringComparer.OrdinalIgnoreCase)
    {
        "/api/v1/books",
        "/api/v1/members"
    };

    public AuthorizationMiddleware(RequestDelegate next, ILogger<AuthorizationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IRoleAuthorizationService roleAuthService)
    {
        var path = context.Request.Path.Value ?? string.Empty;
        var method = context.Request.Method;

        // Check if the route is public
        if (IsPublicRoute(path, method))
        {
            _logger.LogInformation($"Public route accessed: {method} {path}");
            await _next(context);
            return;
        }

        // For protected routes, verify user is authenticated and authorized
        var user = context.User;
        
        if (user?.Identity?.IsAuthenticated != true)
        {
            _logger.LogWarning($"Unauthorized access attempt to {method} {path} - No authentication");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new
            {
                success = false,
                message = "Unauthorized - No valid authentication token provided",
                statusCode = StatusCodes.Status401Unauthorized
            });
            return;
        }

        // Extract user ID from claims
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
        {
            _logger.LogWarning($"Invalid user ID in token for {path}");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new
            {
                success = false,
                message = "Unauthorized - Invalid user information",
                statusCode = StatusCodes.Status401Unauthorized
            });
            return;
        }

        // Get user role from database
        var userRole = await roleAuthService.GetUserRoleAsync(userId);
        
        if (string.IsNullOrEmpty(userRole))
        {
            _logger.LogWarning($"Could not retrieve role for user {userId}");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new
            {
                success = false,
                message = "Unauthorized - User role could not be verified",
                statusCode = StatusCodes.Status401Unauthorized
            });
            return;
        }

        // Check if route requires Librarian/Admin role
        if (RequiresLibrarianRole(path, method))
        {
            var isLibrarianOrAdmin = await roleAuthService.IsLibrarianOrAdminAsync(userId);
            
            if (!isLibrarianOrAdmin)
            {
                _logger.LogWarning($"Forbidden access attempt by user {userId} (Role: {userRole}) to {method} {path}");
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new
                {
                    success = false,
                    message = "Forbidden - Only Librarian and Admin users can access this resource",
                    statusCode = StatusCodes.Status403Forbidden
                });
                return;
            }
        }

        // Log authorized access
        var username = user.FindFirst(ClaimTypes.Name)?.Value;
        _logger.LogInformation($"Authorized access: User={username} (ID={userId}), Role={userRole}, Method={method}, Path={path}");

        // Add user info to context items for controllers to use
        context.Items["UserId"] = userId;
        context.Items["Username"] = username;
        context.Items["UserRole"] = userRole;

        await _next(context);
    }

    /// <summary>
    /// Check if route is public (doesn't require authentication)
    /// </summary>
    private static bool IsPublicRoute(string path, string method)
    {
        // Check exact matches
        if (PublicRoutes.Contains(path))
            return true;

        // Check if path matches any public route pattern
        foreach (var publicRoute in PublicRoutes)
        {
            if (path.Equals(publicRoute, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        // Health check endpoints (case-insensitive)
        if (path.StartsWith("/health", StringComparison.OrdinalIgnoreCase))
            return true;

        return false;
    }

    /// <summary>
    /// Check if route requires Librarian or Admin role for write operations (POST, PUT, PATCH, DELETE)
    /// </summary>
    private static bool RequiresLibrarianRole(string path, string method)
    {
        // Only POST, PUT, PATCH, DELETE require librarian role
        if (!method.Equals("POST", StringComparison.OrdinalIgnoreCase) &&
            !method.Equals("PUT", StringComparison.OrdinalIgnoreCase) &&
            !method.Equals("PATCH", StringComparison.OrdinalIgnoreCase) &&
            !method.Equals("DELETE", StringComparison.OrdinalIgnoreCase))
        {
            return false; // GET requests allowed for all authenticated users
        }

        // Check if path starts with any librarian-only prefix
        foreach (var prefix in LibrarianOnlyPrefixes)
        {
            if (path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }
}
