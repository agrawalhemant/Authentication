using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Authentication.API.Controllers;

[ApiController]
public class HealthController : ControllerBase
{
    public HealthController()
    {
        
    }
    
    [DisableRateLimiting]
    [Route("")]
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Authentication API is up and running.");
    }
}