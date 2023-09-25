using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using PNGame.ActionFilters;
using PNUnity.Share;

namespace PNGame.Controllers;

[Route("[controller]/[action]")]
[FileLogFilter]
public class PNGameController : ControllerBase
{
    protected static IActionResult Error(ErrorCode ec, [CallerFilePath] string filepath = "", [CallerLineNumber] int line = 0)
    {
        return new JsonResult(new
        {
            code = (int)ec,
            debug = $"{ec} {Path.GetFileName(filepath)}:{line}",
        });
    }

    protected static IActionResult Success(object response)
    {
        return new JsonResult(response);
    }
}