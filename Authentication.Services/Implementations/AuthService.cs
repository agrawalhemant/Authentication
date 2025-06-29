using Authentication.Contracts.DTOs;
using Authentication.DAL.Interfaces;
using Authentication.DAL.Models;
using Authentication.Services.Interfaces;
using AutoMapper;

namespace Authentication.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailVerificationRepository _emailVerificationRepository;
    private readonly IPhoneVerificationRepository _phoneVerificationRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    public AuthService(IUserRepository userRepository, IEmailVerificationRepository emailVerificationRepository, IPhoneVerificationRepository phoneVerificationRepository, IPasswordHasher passwordHasher, ITokenService tokenService, IMapper mapper)
    {
        _userRepository = userRepository;
        _emailVerificationRepository = emailVerificationRepository;
        _phoneVerificationRepository = phoneVerificationRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _mapper = mapper;
    }

    public async Task<RegisterResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var existingUser = await _userRepository.GetUserByEmailAsync(request.Email, cancellationToken);

        if (existingUser != null) 
            throw new Exception("Email already registered");

        var passwordHash = _passwordHasher.Hash(request.Password);

        var user = _mapper.Map<User>(request);
        user.passwordhash = passwordHash;
        await _userRepository.AddUserAsync(user, cancellationToken);

        return new RegisterResponse
        {
            UserId = user.id,
            Message = "Registration successful"
        };
    }

    public async Task<(SecurityToken,LoginResponse?)> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetUserByEmailAsync(request.Email, cancellationToken);
        if (user == null || !_passwordHasher.Verify(request.Password, user.passwordhash))
            throw new UnauthorizedAccessException("Either email or password is incorrect.");
        
        if (!user.isactive)
            throw new Exception("User is inactive.");

        LoginResponse loginResponse = new LoginResponse
        {
            UserId = user.id,
            Email = user.email,
            FirstName = user.firstname,
            LastName = user.lastname,
        };
        
        var accessToken = _tokenService.GenerateAccessToken(user.id.ToString(), user.role);

        SecurityToken securityTokens = new SecurityToken
        {
            AccessToken = accessToken,
        };
        
        return (securityTokens, loginResponse);
    }

    public async Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken cancellationToken = default)
    {
        if (request.CurrentPassword == request.NewPassword)
            throw new ArgumentException("Current password and new password can't be same.");
        
        if (request.NewPassword != request.ConfirmPassword)
            throw new ArgumentException("New password and confirmation password do not match.");

        var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken);
        if (user == null)
            throw new KeyNotFoundException("User not found.");

        if (!_passwordHasher.Verify(request.CurrentPassword, user.passwordhash))
        {
            throw new UnauthorizedAccessException("Current password is incorrect.");
        }
        
        user.passwordhash = _passwordHasher.Hash(request.NewPassword);
        user.updatedat = DateTime.UtcNow;

        await _userRepository.UpdateUserAsync(user, cancellationToken);
    }

    public async Task ChangeEmailAsync(Guid userId, ChangeEmailRequest request, CancellationToken cancellationToken = default)
    {
        if (request.NewEmail != request.ConfirmEmail)
            throw new ArgumentException("New email and confirmation email do not match.");

        bool existingEmail = await _userRepository.UserExistsByEmailAsync(request.NewEmail, cancellationToken);
        if(existingEmail)
            throw new ArgumentException("New email already exists with another user. Please try another email.");
        
        var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken);
        if (user == null)
            throw new KeyNotFoundException("User not found.");

        if(user.email == request.NewEmail)
            throw new ArgumentException("Current email & new email can't be same.");

        user.email = request.NewEmail;
        user.isemailverified = false;
        user.updatedat = DateTime.UtcNow;

        await _userRepository.UpdateUserAsync(user, cancellationToken);
        await _emailVerificationRepository.ExpireAllCodesByUserIdAsync(userId, cancellationToken);
    }

    public async Task UpdatePhoneAsync(Guid userId, string phone, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken);
        if (user == null)
            throw new KeyNotFoundException("User not found.");
        user.phonenumber = phone;
        user.isphoneverified = false;
        user.updatedat = DateTime.UtcNow;
        await _userRepository.UpdateUserAsync(user, cancellationToken);
        await _phoneVerificationRepository.ExpireAllCodesByUserIdAsync(userId, cancellationToken);
    }
}