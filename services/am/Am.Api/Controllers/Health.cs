using Microsoft.AspNetCore.Mvc;

namespace Am.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class Health : Controller
{
    [HttpGet("wakeup")]
    public IActionResult WakeUp()
    {
        return Ok();
    }
}