using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using PNShare.DB;
using PNShare.Global;
using PNUnity.Share;

namespace PNGame.Modules;

public static partial class GModule
{
    private static (ModuleError me, MongoGamePlayer1 mp) Error(MongoResult mr, [CallerFilePath] string filepath = "", [CallerLineNumber] int line = 0) => (mr.ToModuleError(filepath, line), default);
    private static (ModuleError me, MongoGamePlayer1 mp) Error(ErrorCode ec, [CallerFilePath] string filepath = "", [CallerLineNumber] int line = 0) => (ec.ToModuleError(filepath, line), default);

    public static async Task<(ModuleError me, MongoGamePlayer1 mp)> Load_Player_UpdateTokenExpire(string token)
    {
        (var mr, var mp) = await GameDB.Player.Select(token);
        if (mr != MongoResult.SUCCESS)
            return Error(mr);
        if (null == mp)
            return Error(ErrorCode.TOKEN_NOPLAYER);

        var now = GTime.Now();
        if (mp.tokenexpire <= now)
            return Error(ErrorCode.TOKEN_EXPIRED);

        var tokenexpire = now.AddHours(1);
        mr = await GameDB.Player.Update(mp.id, u => u.Set(p => p.tokenexpire, tokenexpire));
        if (mr != MongoResult.SUCCESS)
            return Error(mr);

        return (default, mp);
    }
}