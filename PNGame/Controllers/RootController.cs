using Microsoft.AspNetCore.Mvc;

namespace PNGame.Controllers;

[Route("[action]")]
public class RootController : ControllerBase
{
    public IActionResult Health() => Ok(new { });
}