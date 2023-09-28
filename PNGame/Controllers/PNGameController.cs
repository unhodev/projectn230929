using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using PNGame.ActionFilters;
using PNGame.Modules;
using PNShare.DB;
using PNShare.Global;
using PNUnity.Share;

namespace PNGame.Controllers;

[Route("[controller]/[action]")]
[FileLogFilter]
public class PNGameController : ControllerBase
{
    protected static string Error(ErrorCode ec, [CallerFilePath] string filepath = "", [CallerLineNumber] int line = 0)
    {
        return GJson.SerializeObject(new
        {
            result = (int)ec,
            debug = $"{ec} {Path.GetFileName(filepath)}:{line}",
        });
    }

    protected static string Error(MongoResult mr, [CallerFilePath] string filepath = "", [CallerLineNumber] int line = 0)
    {
        return GJson.SerializeObject(new
        {
            result = (int)ErrorCode.DB_ERROR,
            debug = $"{mr} {Path.GetFileName(filepath)}:{line}",
        });
    }

    protected static string Error(ModuleError me)
    {
        return GJson.SerializeObject(new
        {
            result = (int)me.code,
            debug = $"{me.message} {Path.GetFileName(me.filepath)}:{me.line}",
        });
    }

    protected static string Success(object response)
    {
        return GJson.SerializeObject(response);
    }
}