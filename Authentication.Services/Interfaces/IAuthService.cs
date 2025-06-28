using Authentication.Contracts.DTOs;

namespace Authentication.Services.Interfaces;

public interface IAuthService
{
    Task<RegisterResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<(SecurityToken, LoginResponse?)> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken cancellationToken = default);
    Task ChangeEmailAsync(Guid userId, ChangeEmailRequest request, CancellationToken cancellationToken = default);
}