using Microsoft.AspNetCore.Mvc;

namespace Rdm.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class GetResult : Controller
{

    public GetResult()
    {
        
    }
    
    [HttpGet("/test")]
    public IActionResult Test()
    {
        return Ok();
    }
}