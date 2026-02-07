namespace LibraryManagementSystem.Services.Implementations;

using Microsoft.EntityFrameworkCore;
using Data;
using Models;
using Repository.Interfaces;

/// <summary>
/// User repository implementation using Entity Framework Core
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(ApplicationDbContext context, ILogger<UserRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        try
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id && u.IsActive);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting user by ID: {ex.Message}");
            return null;
        }
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        try
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting user by username: {ex.Message}");
            return null;
        }
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        try
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting user by email: {ex.Message}");
            return null;
        }
    }

    public async Task<User> CreateUserAsync(User user)
    {
        try
        {
            user.CreatedAt = DateTime.UtcNow;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"User created: {user.Username}");
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating user: {ex.Message}");
            throw;
        }
    }

    public async Task<User?> UpdateUserAsync(User user)
    {
        try
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
            if (existingUser == null)
                return null;

            user.UpdatedAt = DateTime.UtcNow;
            _context.Entry(existingUser).CurrentValues.SetValues(user);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"User updated: {user.Username}");
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating user: {ex.Message}");
            throw;
        }
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
                return false;

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            _logger.LogInformation($"User deleted (soft): {id}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting user: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        try
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error checking username existence: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        try
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error checking email existence: {ex.Message}");
            return false;
        }
    }
}

/// <summary>
/// Refresh token repository implementation using Entity Framework Core
/// </summary>
public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<RefreshTokenRepository> _logger;

    public RefreshTokenRepository(ApplicationDbContext context, ILogger<RefreshTokenRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<RefreshToken?> GetTokenAsync(string token)
    {
        try
        {
            return await _context.RefreshTokens
                .AsNoTracking()
                .FirstOrDefaultAsync(rt => rt.Token == token && !rt.IsRevoked && rt.ExpiresAt > DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting refresh token: {ex.Message}");
            return null;
        }
    }

    public async Task<RefreshToken> CreateTokenAsync(RefreshToken refreshToken)
    {
        try
        {
            refreshToken.CreatedAt = DateTime.UtcNow;
            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Refresh token created for user: {refreshToken.UserId}");
            return refreshToken;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating refresh token: {ex.Message}");
            throw;
        }
    }

    public async Task<bool> RevokeTokenAsync(string token)
    {
        try
        {
            var refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);
            if (refreshToken == null)
                return false;

            refreshToken.IsRevoked = true;
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Refresh token revoked");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error revoking token: {ex.Message}");
            return false;
        }
    }

    public async Task<List<RefreshToken>> GetUserTokensAsync(int userId)
    {
        try
        {
            return await _context.RefreshTokens
                .AsNoTracking()
                .Where(rt => rt.UserId == userId && !rt.IsRevoked)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting user tokens: {ex.Message}");
            return new List<RefreshToken>();
        }
    }

    public async Task<int> CleanupExpiredTokensAsync()
    {
        try
        {
            var expiredTokens = await _context.RefreshTokens
                .Where(rt => rt.ExpiresAt <= DateTime.UtcNow)
                .ToListAsync();

            _context.RefreshTokens.RemoveRange(expiredTokens);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Cleaned up {expiredTokens.Count} expired tokens");
            return expiredTokens.Count;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error cleaning up expired tokens: {ex.Message}");
            return 0;
        }
    }
}
