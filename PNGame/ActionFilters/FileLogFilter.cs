using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PNShare.Global;

namespace PNGame.ActionFilters;

public class FileLogFilter : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context1, ActionExecutionDelegate next)
    {
        var connectionid = context1.HttpContext.Connection.Id;
        var path = context1.HttpContext.Request.Path.ToString()[1..];

        var reqdatas = GetReqDatas(context1.HttpContext.Request);
        GLog.Write($"{path} {connectionid} > {JsonSerializer.Serialize(reqdatas)}");

        var context2 = await next();

        var resdatas = GetResDatas(context2.Result);
        GLog.Write($"{path} {connectionid} < {resdatas}");
    }

    private static Dictionary<string, string> GetReqDatas(HttpRequest req)
    {
        var map = new Dictionary<string, string>();
        foreach (var kv in req.Query)
            map[kv.Key] = kv.Value.ToString();
        try
        {
            foreach (var kv in req.Form)
                map[kv.Key] = kv.Value.ToString();
        }
        catch
        {
            // ignored
        }

        return map;
    }

    private static string GetResDatas(IActionResult result) => result switch
    {
        JsonResult { Value: { } value } => GJson.SerializeObject(value),
        ObjectResult { Value: string value } => value,
        _ => "null",
    };
}