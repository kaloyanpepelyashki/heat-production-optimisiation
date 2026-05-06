using Microsoft.AspNetCore.Mvc;

namespace Sdm.Api.Controllers;


[Microsoft.AspNetCore.Components.Route("api/[controller]")]
[ApiController]
public class Health : Controller
{
    [HttpGet("wakeup")]
    public IActionResult WakeUp()
    {
        return Ok();
    }
}