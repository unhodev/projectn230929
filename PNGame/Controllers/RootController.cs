using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PNShare.DB;
using PNUnity.Share;

namespace PNGame.Controllers;

[Route("[action]")]
public class RootController : ControllerBase
{
    public async Task<IActionResult> Health()
    {
        await GameDB.Ping();
        return Ok(new { result = (int)ErrorCode.SUCCESS });
    }
}