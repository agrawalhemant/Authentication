using Authentication.Contracts.DTOs;
using Authentication.Contracts.Pagination;

namespace Authentication.Services.Interfaces;

public interface IUserService
{
    Task<UserDto?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserDto?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<PageResult<UserDto>> GetUsersAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}