using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [Authorize] // This requires JWT
    [HttpGet("protected")]
    public IActionResult Protected()
    {
        return Ok(new
        {
            message = "JWT works!",
            user = User.Identity.Name,
            email = User.FindFirst(ClaimTypes.Email)?.Value
        });
    }

    [AllowAnonymous]
    [HttpGet("public")]
    public IActionResult Public() => Ok(new { message = "No JWT needed" });
}