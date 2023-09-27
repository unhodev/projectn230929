using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using PNGame.ActionFilters;
using PNShare.DB;
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
            result = (int)ec,
            debug = $"{ec} {Path.GetFileName(filepath)}:{line}",
        });
    }

    protected static IActionResult Error(MongoResult mr, [CallerFilePath] string filepath = "", [CallerLineNumber] int line = 0)
    {
        return new JsonResult(new
        {
            result = (int)ErrorCode.DB_ERROR,
            debug = $"{mr} {Path.GetFileName(filepath)}:{line}",
        });
    }

    protected static IActionResult Success(object response)
    {
        return new JsonResult(response);
    }
}