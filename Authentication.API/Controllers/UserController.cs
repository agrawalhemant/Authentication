using Authentication.DAL.Interfaces;
using Authentication.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        
        /// <summary>
        /// Get user details by userId
        /// </summary>
        /// <returns></returns>
        [HttpGet("{userId}", Name = "GetUserDetails")]
        public async Task<IActionResult> GetUserDetailsAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(userId, cancellationToken);
                if (user is not null)
                    return Ok(user);
                return NotFound("No user found with the given userId.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        /// <summary>
        /// Get all active users
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin, User")]
        [HttpGet("all", Name = "GetAllUserDetails")]
        public async Task<IActionResult> GetAllUserDetailsAsync(int pageNumber = 1, int size = 10, CancellationToken cancellationToken = default)
        {
            try
            {
                if(pageNumber <=0)
                    pageNumber = 1;
                if(size <=0)
                    size = 10;
                
                var res = await _userService.GetUsersAsync(pageNumber, size, cancellationToken);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
