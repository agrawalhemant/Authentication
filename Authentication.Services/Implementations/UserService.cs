using Authentication.Contracts.DTOs;
using Authentication.Contracts.Pagination;
using Authentication.DAL.Interfaces;
using Authentication.DAL.Models;
using Authentication.Services.Interfaces;
using AutoMapper;

namespace Authentication.Services.Implementations;

public class UserService : IUserService
{
    private IUserRepository _userRepository;
    private IMapper _mapper;
    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }


    public async Task<UserDto?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        User? user = await _userRepository.GetUserByIdAsync(userId, cancellationToken);
        UserDto userDto = _mapper.Map<UserDto>(user);
        return userDto;
    }

    public async Task<UserDto?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        User? user = await _userRepository.GetUserByEmailAsync(email, cancellationToken);
        UserDto userDto = _mapper.Map<UserDto>(user);
        return userDto;
    }

    public async Task<PageResult<UserDto>> GetUsersAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        PageResult<UserDto> pageResult = new PageResult<UserDto>();
        int totalUsers = await _userRepository.GetUsersCountAsync(cancellationToken);
        if (totalUsers == 0)
        {
            return pageResult;
        }
        
        List<User> users = await _userRepository.GetUsersAsync(pageNumber, pageSize, cancellationToken);
        pageResult.TotalCount = totalUsers;
        pageResult.PageSize = pageSize;
        pageResult.CurrentPage = pageNumber;
        pageResult.Result = _mapper.Map<List<UserDto?>>(users);
        return pageResult;
    }
}