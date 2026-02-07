namespace LibraryManagementSystem.Services.Implementations;

using Data;
using Interfaces;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Implementation of role-based authorization service
/// </summary>
public class RoleAuthorizationService : IRoleAuthorizationService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<RoleAuthorizationService> _logger;

    public RoleAuthorizationService(ApplicationDbContext context, ILogger<RoleAuthorizationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Check if user has specific role
    /// </summary>
    public async Task<bool> HasRoleAsync(int userId, string role)
    {
        try
        {
            var userRole = await GetUserRoleAsync(userId);
            return userRole?.Equals(role, StringComparison.OrdinalIgnoreCase) ?? false;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error checking role for user {userId}: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Check if user has any of the specified roles
    /// </summary>
    public async Task<bool> HasAnyRoleAsync(int userId, params string[] roles)
    {
        try
        {
            var userRole = await GetUserRoleAsync(userId);
            return roles.Any(r => r.Equals(userRole, StringComparison.OrdinalIgnoreCase));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error checking roles for user {userId}: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Get user role from database
    /// </summary>
    public async Task<string?> GetUserRoleAsync(int userId)
    {
        try
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive);
            
            if (user == null)
            {
                _logger.LogWarning($"User {userId} not found or inactive");
                return null;
            }

            _logger.LogInformation($"Retrieved role '{user.Role}' for user {userId}");
            return user.Role;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting role for user {userId}: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Check if user is librarian or admin
    /// </summary>
    public async Task<bool> IsLibrarianOrAdminAsync(int userId)
    {
        return await HasAnyRoleAsync(userId, "Librarian", "Admin");
    }

    /// <summary>
    /// Check if user is admin
    /// </summary>
    public async Task<bool> IsAdminAsync(int userId)
    {
        return await HasRoleAsync(userId, "Admin");
    }
}
