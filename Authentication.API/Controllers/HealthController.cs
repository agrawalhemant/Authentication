using System.Threading.Tasks;
using Authentication.DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

namespace Authentication.API.Controllers;

[ApiController]
public class HealthController : ControllerBase
{
    private readonly AuthDbContext _dbContext;

    public HealthController(AuthDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    [DisableRateLimiting]
    [Route("")]
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        if (!await _dbContext.Database.CanConnectAsync())
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new
            {
                status = "Unhealthy",
                message = "Unable to connect to the database."
            });
        }

        return Ok(new
        {
            status = "Healthy",
            message = "Authentication API is up and running.",
            database = "Connected"
        });
    }
}