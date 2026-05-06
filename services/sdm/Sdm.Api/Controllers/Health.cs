using Microsoft.AspNetCore.Mvc;

namespace Sdm.Api.Controllers;


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