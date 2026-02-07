namespace LibraryManagementSystem.Models.DTOs;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// DTO for user registration
/// </summary>
public class RegisterUserDto
{
    [Required(ErrorMessage = "Username is required")]
    [StringLength(256, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 256 characters")]
    public string Username { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Password is required")]
    [StringLength(256, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
    public string Password { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Full name is required")]
    [StringLength(256)]
    public string FullName { get; set; } = string.Empty;
}

/// <summary>
/// DTO for user login
/// </summary>
public class LoginDto
{
    [Required(ErrorMessage = "Username or email is required")]
    public string UsernameOrEmail { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// DTO for refresh token request
/// </summary>
public class RefreshTokenDto
{
    [Required(ErrorMessage = "Access token is required")]
    public string AccessToken { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Refresh token is required")]
    public string RefreshToken { get; set; } = string.Empty;
}

/// <summary>
/// DTO for authentication response
/// </summary>
public class AuthenticationResponseDto
{
    public bool Success { get; set; }
    
    public string Message { get; set; } = string.Empty;
    
    public string? AccessToken { get; set; }
    
    public string? RefreshToken { get; set; }
    
    public UserDto? User { get; set; }
}

/// <summary>
/// DTO for user response
/// </summary>
public class UserDto
{
    public int Id { get; set; }
    
    public string Username { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    
    public string FullName { get; set; } = string.Empty;
    
    public string Role { get; set; } = string.Empty;
    
    public bool IsActive { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? LastLogin { get; set; }
}

/// <summary>
/// DTO for assigning a role to a user (Admin only)
/// </summary>
public class AssignRoleDto
{
    [Required(ErrorMessage = "Username is required")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Role is required")]
    [RegularExpression("^(User|Librarian|Admin)$", ErrorMessage = "Role must be User, Librarian, or Admin")]
    public string Role { get; set; } = string.Empty;
}
