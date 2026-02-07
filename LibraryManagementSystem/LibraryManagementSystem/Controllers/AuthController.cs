namespace LibraryManagementSystem.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs;
using Services.Interfaces;

/// <summary>
/// API controller for authentication operations
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthenticationService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Register a new user account
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<object>> Register([FromBody] RegisterUserDto registerDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _logger.LogInformation($"Registration attempt for: {registerDto.Username}");

        var (success, message) = await _authService.RegisterAsync(registerDto);
        
        if (!success)
            return BadRequest(new { message });

        return StatusCode(StatusCodes.Status201Created, new { message });
    }

    /// <summary>
    /// Authenticate user and return JWT tokens
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthenticationResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _logger.LogInformation($"Login attempt for: {loginDto.UsernameOrEmail}");

        var response = await _authService.LoginAsync(loginDto);

        if (!response.Success)
            return Unauthorized(response);

        return Ok(response);
    }

    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthenticationResponseDto>> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _logger.LogInformation("Token refresh attempt");

        var response = await _authService.RefreshTokenAsync(refreshTokenDto);

        if (!response.Success)
            return Unauthorized(response);

        return Ok(response);
    }

    /// <summary>
    /// Logout by revoking refresh token
    /// </summary>
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<object>> Logout([FromBody] RefreshTokenDto logoutDto)
    {
        if (string.IsNullOrEmpty(logoutDto.RefreshToken))
            return BadRequest(new { message = "Refresh token is required" });

        _logger.LogInformation("Logout attempt");

        var result = await _authService.RevokeTokenAsync(logoutDto.RefreshToken);

        if (!result)
            return BadRequest(new { message = "Failed to revoke token" });

        return Ok(new { message = "Logout successful" });
    }

    /// <summary>
    /// Assign a role to a user (Admin only)
    /// </summary>
    [HttpPut("assign-role")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<object>> AssignRole([FromBody] AssignRoleDto assignRoleDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _logger.LogInformation($"Role assignment attempt: {assignRoleDto.Username} -> {assignRoleDto.Role}");

        var (success, message) = await _authService.AssignRoleAsync(assignRoleDto);

        if (!success)
            return BadRequest(new { message });

        return Ok(new { message });
    }
}
