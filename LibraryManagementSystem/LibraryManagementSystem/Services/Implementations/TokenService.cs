namespace LibraryManagementSystem.Services.Implementations;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Models;
using Interfaces;
using Microsoft.IdentityModel.Tokens;

/// <summary>
/// Implementation of JWT token service
/// </summary>
public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<TokenService> _logger;

    public TokenService(IConfiguration configuration, ILogger<TokenService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Generate JWT access token
    /// </summary>
    public string GenerateAccessToken(User user)
    {
        try
        {
            var key = _configuration["Jwt:SecretKey"];
            if (string.IsNullOrEmpty(key))
                throw new InvalidOperationException("JWT secret key not configured");

            var keyBytes = Encoding.ASCII.GetBytes(key);
            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(keyBytes),
                SecurityAlgorithms.HmacSha256
            );

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.FullName),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var expiresIn = int.TryParse(_configuration["Jwt:ExpiresInMinutes"], out var minutes) 
                ? minutes 
                : 60;

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expiresIn),
                SigningCredentials = signingCredentials,
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);

            _logger.LogInformation($"Access token generated for user: {user.Username}");
            return token;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error generating access token: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Generate refresh token
    /// </summary>
    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }

    /// <summary>
    /// Validate JWT token
    /// </summary>
    public bool ValidateToken(string token)
    {
        try
        {
            var key = _configuration["Jwt:SecretKey"];
            if (string.IsNullOrEmpty(key))
                return false;

            var keyBytes = Encoding.ASCII.GetBytes(key);
            var tokenHandler = new JwtSecurityTokenHandler();

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return validatedToken is JwtSecurityToken;
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"Token validation failed: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Extract user ID from token
    /// </summary>
    public int? GetUserIdFromToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            if (jwtToken == null)
                return null;

            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => 
                c.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
                return userId;

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error extracting user ID from token: {ex.Message}");
            return null;
        }
    }
}
