using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Authentication.Contracts.Config;
using Authentication.Contracts.DTOs;
using Authentication.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
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
    private readonly IPhoneService _phoneService;

    public AuthController(IAuthService authService, IEmailService emailService, ITokenService tokenService, IPhoneService phoneService, IOptions<JwtSettings> jwtOptions)
    {
        _authService = authService;
        _tokenService = tokenService;
        _emailService = emailService;
        _phoneService = phoneService;
        _jwtSettings = jwtOptions.Value;
    }
    
    /// <summary>
    /// Register user
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [DisableRateLimiting]
    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            if (request is null)
            {
                return BadRequest("invalid request body. Please provide valid request body.");
            }
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
    [DisableRateLimiting]
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (request is null)
            {
                return BadRequest("invalid request body. Please provide valid request body.");
            }
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
    [DisableRateLimiting]
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> LogoutAsync()
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
    [DisableRateLimiting]
    [Authorize]
    [HttpPut("change-email")]
    public async Task<IActionResult> ChangeEmailAsync([FromBody] ChangeEmailRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (request is null)
            {
                return BadRequest("invalid request body. Please provide valid request body.");
            }
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
    [DisableRateLimiting]
    [Authorize]
    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (request is null)
            {
                return BadRequest("invalid request body. Please provide valid request body.");
            }
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
    [HttpPost("email/send-verification-code")]
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
    [DisableRateLimiting]
    [Authorize]
    [HttpPost("email/verify-code")]
    public async Task<IActionResult> VerifyEmailAsync([Required] string code, CancellationToken cancellationToken = default)
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

    /// <summary>
    /// Add or update a phone number (Phone number should be in the format of country code followed by phone number)
    /// </summary>
    /// <param name="phoneNumber"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPut("phone/add")]
    public async Task<IActionResult> AddOrUpdatePhoneNumberAsync([Required] string phoneNumber, bool sendVerificationSms = false,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized("Invalid user ID.");
        
            if(!Regex.IsMatch(phoneNumber, @"^\+[1-9]\d+$"))
                return BadRequest("Phone number should be in the format of country code followed by phone number.");
            
            await _authService.UpdatePhoneAsync(userId, phoneNumber, cancellationToken);
            if (sendVerificationSms)
            {
                await _phoneService.SendVerificationSmsAsync(userId, cancellationToken);
                return Ok(new { message = "Phone number added successfully. Verification code sent to your phone." });
            }
            return Ok(new { message = "Phone number added successfully. Please Verify your phone number." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    /// <summary>
    /// Send verification code to phone number
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPost("phone/send-verification-code")]
    public async Task<IActionResult> SendVerificationSmsAsync(CancellationToken cancellationToken)
    {
        try
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized("Invalid user ID.");
            
            await _phoneService.SendVerificationSmsAsync(userId, cancellationToken);
            return Ok("Verification code sent to your phone");
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    /// <summary>
    /// verify phone number using verification code
    /// </summary>
    /// <param name="code"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [DisableRateLimiting]
    [Authorize]
    [HttpPost("phone/verify-code")]
    public async Task<IActionResult> VerifyPhoneAsync([Required] string code, CancellationToken cancellationToken = default)
    {
        try
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized("Invalid user ID.");
            
            var res = await _phoneService.VerifyPhoneAsync(userId, code, cancellationToken);
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