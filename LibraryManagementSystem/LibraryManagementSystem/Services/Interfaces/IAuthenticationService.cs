namespace LibraryManagementSystem.Services.Interfaces;

using Models;
using Models.DTOs;

/// <summary>
/// Interface for authentication operations
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Register a new user
    /// </summary>
    Task<(bool Success, string Message)> RegisterAsync(RegisterUserDto dto);
    
    /// <summary>
    /// Authenticate user and return JWT tokens
    /// </summary>
    Task<AuthenticationResponseDto> LoginAsync(LoginDto dto);
    
    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
    Task<AuthenticationResponseDto> RefreshTokenAsync(RefreshTokenDto dto);
    
    /// <summary>
    /// Revoke refresh token
    /// </summary>
    Task<bool> RevokeTokenAsync(string refreshToken);
    
    /// <summary>
    /// Verify if username exists
    /// </summary>
    Task<bool> UsernameExistsAsync(string username);
    
    /// <summary>
    /// Verify if email exists
    /// </summary>
    Task<bool> EmailExistsAsync(string email);

    /// <summary>
    /// Assign a role to a user (Admin only)
    /// </summary>
    Task<(bool Success, string Message)> AssignRoleAsync(AssignRoleDto dto);
}

/// <summary>
/// Interface for JWT token operations
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generate JWT access token
    /// </summary>
    string GenerateAccessToken(User user);
    
    /// <summary>
    /// Generate refresh token
    /// </summary>
    string GenerateRefreshToken();
    
    /// <summary>
    /// Validate JWT token
    /// </summary>
    bool ValidateToken(string token);
    
    /// <summary>
    /// Extract user ID from JWT token
    /// </summary>
    int? GetUserIdFromToken(string token);
}
