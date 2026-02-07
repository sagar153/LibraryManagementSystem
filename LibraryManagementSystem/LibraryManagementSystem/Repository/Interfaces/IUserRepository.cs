namespace LibraryManagementSystem.Repository.Interfaces;

using Models;

/// <summary>
/// Interface for user repository operations
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Get user by ID
    /// </summary>
    Task<User?> GetUserByIdAsync(int id);
    
    /// <summary>
    /// Get user by username
    /// </summary>
    Task<User?> GetUserByUsernameAsync(string username);
    
    /// <summary>
    /// Get user by email
    /// </summary>
    Task<User?> GetUserByEmailAsync(string email);
    
    /// <summary>
    /// Create new user
    /// </summary>
    Task<User> CreateUserAsync(User user);
    
    /// <summary>
    /// Update user
    /// </summary>
    Task<User?> UpdateUserAsync(User user);
    
    /// <summary>
    /// Delete user (soft delete)
    /// </summary>
    Task<bool> DeleteUserAsync(int id);
    
    /// <summary>
    /// Check if username exists
    /// </summary>
    Task<bool> UsernameExistsAsync(string username);
    
    /// <summary>
    /// Check if email exists
    /// </summary>
    Task<bool> EmailExistsAsync(string email);
}

/// <summary>
/// Interface for refresh token repository operations
/// </summary>
public interface IRefreshTokenRepository
{
    /// <summary>
    /// Get refresh token by token string
    /// </summary>
    Task<RefreshToken?> GetTokenAsync(string token);
    
    /// <summary>
    /// Create new refresh token
    /// </summary>
    Task<RefreshToken> CreateTokenAsync(RefreshToken refreshToken);
    
    /// <summary>
    /// Revoke refresh token
    /// </summary>
    Task<bool> RevokeTokenAsync(string token);
    
    /// <summary>
    /// Get all refresh tokens for user
    /// </summary>
    Task<List<RefreshToken>> GetUserTokensAsync(int userId);
    
    /// <summary>
    /// Clean up expired tokens
    /// </summary>
    Task<int> CleanupExpiredTokensAsync();
}
