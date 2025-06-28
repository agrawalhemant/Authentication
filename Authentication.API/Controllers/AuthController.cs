using System.Security.Claims;
using Authentication.Contracts.Config;
using Authentication.Contracts.DTOs;
using Authentication.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Authentication.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ITokenService _tokenService;
    private readonly JwtSettings _jwtSettings;
    private readonly IEmailService _emailService;

    public AuthController(IAuthService authService, IEmailService emailService, ITokenService tokenService,IOptions<JwtSettings> jwtOptions)
    {
        _authService = authService;
        _tokenService = tokenService;
        _emailService = emailService;
        _jwtSettings = jwtOptions.Value;
    }
    
    /// <summary>
    /// Register user
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            // Register user
            var response = await _authService.RegisterAsync(request, cancellationToken);
            
            // Login user after registration
            var accessToken = _tokenService.GenerateAccessToken(response.UserId.ToString(), request.Role);
            _tokenService.SetTokenCookie(Response, _jwtSettings.AccessCookie, accessToken, _jwtSettings.ExpireMinutes);

            // Send verification email
            await _emailService.SendWelcomeEmailAsync(request.Email, request.FirstName, cancellationToken);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Login user
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var res = await _authService.LoginAsync(request, cancellationToken);
            if (res.Item2 is null)
                return Unauthorized("Either email or password is incorrect.");
            
            _tokenService.SetTokenCookie(Response, _jwtSettings.AccessCookie, res.Item1.AccessToken, _jwtSettings.ExpireMinutes);
            return Ok(res.Item2);
        }
        catch (Exception ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }
    
    /// <summary>
    /// Logout user. Clears access token cookie.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> LogoutAsync(CancellationToken cancellationToken = default)
    {
        _tokenService.SetTokenCookie(Response, _jwtSettings.AccessCookie, "", -1);
        return NoContent();
    }
    
    /// <summary>
    /// Change Email
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPut("change-email")]
    public async Task<IActionResult> ChangeEmailAsync([FromBody] ChangeEmailRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized("Invalid user ID.");

            await _authService.ChangeEmailAsync(userId, request, cancellationToken);
            return Ok(new { message = "Email changed successfully." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    /// <summary>
    /// Change password
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized("Invalid user ID.");

            await _authService.ChangePasswordAsync(userId, request, cancellationToken);
            return Ok(new { message = "Password changed successfully." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    /// <summary>
    /// Send verification code to email
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPost("email/send-verification")]
    public async Task<IActionResult> SendVerificationEmailAsync(CancellationToken cancellationToken)
    {
        try
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized("Invalid user ID.");
            
            await _emailService.SendVerificationEmailAsync(userId, cancellationToken);
            return Ok("Verification code sent to your email");
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    /// <summary>
    /// Verify email using verification code.
    /// </summary>
    /// <param name="code"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPost("email/verify-code")]
    public async Task<IActionResult> VerifyEmailAsync(string code, CancellationToken cancellationToken = default)
    {
        try
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized("Invalid user ID.");
            
            var res = await _emailService.VerifyEmailAsync(userId, code, cancellationToken);
            if(res.status)
                return Ok(res.message);
            else
                return BadRequest(res.message);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}