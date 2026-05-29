namespace Sdm.Api.Controllers;

using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class Health : Controller
{
    [HttpGet("wakeup")]
    public IActionResult WakeUp()
    {
        return this.Ok();
    }
}