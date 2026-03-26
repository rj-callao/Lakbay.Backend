using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Lakbay.Application.DTOs.Auth;
using Lakbay.Application.Interfaces;

namespace Lakbay.Api.Controllers;

[Route("api/v1/auth")]
public class AuthController : AppBaseController
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        
        if (result == null)
            return Unauthorized(new { error = new { message = "Invalid email or password" } });
        
        return Ok(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        
        if (result == null)
            return BadRequest(new { error = new { message = "Email already exists" } });
        
        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var result = await _authService.RefreshTokenAsync(request.RefreshToken, ipAddress);
        
        if (result == null)
            return Unauthorized(new { error = new { message = "Invalid or expired refresh token" } });
        
        return Ok(result);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var userId = GetCurrentUserId();
        await _authService.LogoutAsync(userId);
        return Ok(new { message = "Logged out successfully" });
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult GetCurrentUser()
    {
        return Ok(new
        {
            Id = GetCurrentUserId(),
            Email = GetCurrentUserEmail(),
            Role = GetCurrentUserRole()
        });
    }
}
