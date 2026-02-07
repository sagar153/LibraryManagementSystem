namespace LibraryManagementSystem.Services.Interfaces;

/// <summary>
/// Interface for role-based authorization
/// </summary>
public interface IRoleAuthorizationService
{
    /// <summary>
    /// Check if user has specific role
    /// </summary>
    Task<bool> HasRoleAsync(int userId, string role);
    
    /// <summary>
    /// Check if user has any of the specified roles
    /// </summary>
    Task<bool> HasAnyRoleAsync(int userId, params string[] roles);
    
    /// <summary>
    /// Get user role from database
    /// </summary>
    Task<string?> GetUserRoleAsync(int userId);
    
    /// <summary>
    /// Check if user is librarian or admin
    /// </summary>
    Task<bool> IsLibrarianOrAdminAsync(int userId);
    
    /// <summary>
    /// Check if user is admin
    /// </summary>
    Task<bool> IsAdminAsync(int userId);
}
