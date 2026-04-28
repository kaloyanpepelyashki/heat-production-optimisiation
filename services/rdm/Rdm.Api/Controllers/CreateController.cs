using Microsoft.AspNetCore.Mvc;

namespace Rdm.Api.Controllers;

[Route("api/[controller]")]
public class CreateController : Controller
{
    public CreateController()
    {
        
    }
    
    [HttpPost("/optimisation")]
    public async Task<IActionResult> RequestOptimisation()
    {
        return Ok(); 
    }
}