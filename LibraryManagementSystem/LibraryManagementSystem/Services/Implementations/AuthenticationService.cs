namespace LibraryManagementSystem.Services.Implementations;

using System.Security.Cryptography;
using System.Text;
using Models;
using Models.DTOs;
using Interfaces;
using LibraryManagementSystem.Repository.Interfaces;

/// <summary>
/// Implementation of authentication service using database
/// </summary>
public class AuthenticationService : IAuthenticationService
{
    private readonly ITokenService _tokenService;
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(
        ITokenService tokenService,
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        ILogger<AuthenticationService> logger)
    {
        _tokenService = tokenService;
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _logger = logger;
    }

    /// <summary>
    /// Register a new user with validation
    /// </summary>
    public async Task<(bool Success, string Message)> RegisterAsync(RegisterUserDto dto)
    {
        try
        {
            // Validate input
            if (await _userRepository.UsernameExistsAsync(dto.Username))
                return (false, "Username already exists");

            if (await _userRepository.EmailExistsAsync(dto.Email))
                return (false, "Email already registered");

            // Create new user
            var (passwordHash, passwordSalt) = HashPassword(dto.Password);
            
            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                FullName = dto.FullName,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Role = "User",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.CreateUserAsync(user);
            _logger.LogInformation($"User registered successfully: {user.Username}");
            
            return (true, "User registered successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error registering user: {ex.Message}");
            return (false, "An error occurred during registration");
        }
    }

    /// <summary>
    /// Authenticate user and return tokens
    /// </summary>
    public async Task<AuthenticationResponseDto> LoginAsync(LoginDto dto)
    {
        try
        {
            // Find user by username or email
            var user = await _userRepository.GetUserByUsernameAsync(dto.UsernameOrEmail) ??
                       await _userRepository.GetUserByEmailAsync(dto.UsernameOrEmail);

            if (user == null || !user.IsActive)
            {
                _logger.LogWarning($"Login attempt with invalid credentials: {dto.UsernameOrEmail}");
                return new AuthenticationResponseDto
                {
                    Success = false,
                    Message = "Invalid username/email or password"
                };
            }

            // Verify password
            if (!VerifyPassword(dto.Password, user.PasswordHash, user.PasswordSalt))
            {
                _logger.LogWarning($"Failed login attempt for user: {user.Username}");
                return new AuthenticationResponseDto
                {
                    Success = false,
                    Message = "Invalid username/email or password"
                };
            }

            // Update last login
            user.LastLogin = DateTime.UtcNow;
            await _userRepository.UpdateUserAsync(user);

            // Generate tokens
            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            // Store refresh token in database
            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };
            await _refreshTokenRepository.CreateTokenAsync(refreshTokenEntity);

            _logger.LogInformation($"User logged in successfully: {user.Username}");

            return new AuthenticationResponseDto
            {
                Success = true,
                Message = "Login successful",
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                User = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
                    Role = user.Role,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    LastLogin = user.LastLogin
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error during login: {ex.Message}");
            return new AuthenticationResponseDto
            {
                Success = false,
                Message = "An error occurred during login"
            };
        }
    }

    /// <summary>
    /// Refresh access token
    /// </summary>
    public async Task<AuthenticationResponseDto> RefreshTokenAsync(RefreshTokenDto dto)
    {
        try
        {
            // Validate refresh token
            var refreshToken = await _refreshTokenRepository.GetTokenAsync(dto.RefreshToken);

            if (refreshToken == null)
            {
                _logger.LogWarning("Invalid or expired refresh token");
                return new AuthenticationResponseDto
                {
                    Success = false,
                    Message = "Invalid or expired refresh token"
                };
            }

            // Get user
            var user = await _userRepository.GetUserByIdAsync(refreshToken.UserId);
            if (user == null || !user.IsActive)
            {
                _logger.LogWarning("User not found for refresh token");
                return new AuthenticationResponseDto
                {
                    Success = false,
                    Message = "User not found"
                };
            }

            // Generate new tokens
            var newAccessToken = _tokenService.GenerateAccessToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            // Revoke old refresh token
            await _refreshTokenRepository.RevokeTokenAsync(refreshToken.Token);

            // Store new refresh token
            var newRefreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                Token = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };
            await _refreshTokenRepository.CreateTokenAsync(newRefreshTokenEntity);

            _logger.LogInformation($"Tokens refreshed for user: {user.Username}");

            return new AuthenticationResponseDto
            {
                Success = true,
                Message = "Token refreshed successfully",
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                User = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
                    Role = user.Role,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    LastLogin = user.LastLogin
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error refreshing token: {ex.Message}");
            return new AuthenticationResponseDto
            {
                Success = false,
                Message = "An error occurred during token refresh"
            };
        }
    }

    /// <summary>
    /// Revoke refresh token
    /// </summary>
    public async Task<bool> RevokeTokenAsync(string refreshToken)
    {
        try
        {
            var result = await _refreshTokenRepository.RevokeTokenAsync(refreshToken);
            if (result)
                _logger.LogInformation($"Refresh token revoked");
            else
                _logger.LogWarning("Failed to revoke token");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error revoking token: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Check if username exists
    /// </summary>
    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await _userRepository.UsernameExistsAsync(username);
    }

    /// <summary>
    /// Check if email exists
    /// </summary>
    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _userRepository.EmailExistsAsync(email);
    }

    /// <summary>
    /// Assign a role to a user (Admin only)
    /// </summary>
    public async Task<(bool Success, string Message)> AssignRoleAsync(AssignRoleDto dto)
    {
        try
        {
            var validRoles = new HashSet<string> { "User", "Librarian", "Admin" };
            if (!validRoles.Contains(dto.Role))
                return (false, "Invalid role. Must be User, Librarian, or Admin");

            var user = await _userRepository.GetUserByUsernameAsync(dto.Username);
            if (user == null)
                return (false, "User not found");

            user.Role = dto.Role;
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateUserAsync(user);

            _logger.LogInformation($"Role '{dto.Role}' assigned to user: {dto.Username}");
            return (true, $"Role '{dto.Role}' assigned to user '{dto.Username}' successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error assigning role: {ex.Message}");
            return (false, "An error occurred while assigning role");
        }
    }

    /// <summary>
    /// Hash password with salt
    /// </summary>
    private static (string hash, string salt) HashPassword(string password)
    {
        using (var rng = new RNGCryptoServiceProvider())
        {
            byte[] saltBuffer = new byte[16];
            rng.GetBytes(saltBuffer);
            string salt = Convert.ToBase64String(saltBuffer);

            var pbkdf2 = new Rfc2898DeriveBytes(password, saltBuffer, 10000, System.Security.Cryptography.HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(20);
            string hashString = Convert.ToBase64String(hash);

            return (hashString, salt);
        }
    }

    /// <summary>
    /// Verify password against hash
    /// </summary>
    private static bool VerifyPassword(string password, string hash, string salt)
    {
        try
        {
            byte[] saltBuffer = Convert.FromBase64String(salt);
            var pbkdf2 = new Rfc2898DeriveBytes(password, saltBuffer, 10000, System.Security.Cryptography.HashAlgorithmName.SHA256);
            byte[] hashBuffer = pbkdf2.GetBytes(20);
            string computedHash = Convert.ToBase64String(hashBuffer);

            return hash == computedHash;
        }
        catch
        {
            return false;
        }
    }
}
