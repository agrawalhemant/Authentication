using Microsoft.AspNetCore.Mvc;

namespace Authentication.API.Controllers;

[ApiController]
public class HealthController : ControllerBase
{
    public HealthController()
    {
        
    }
    [Route("")]
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Authentication API is up and running.");
    }
}