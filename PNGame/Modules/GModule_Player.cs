using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MongoDB.Driver;
using PNShare.DB;
using PNShare.Global;
using PNUnity.Share;

namespace PNGame.Modules;

public static partial class GModule
{
    public static async Task<(ModuleError me, MongoGamePlayer1 mp)> Load_Player_UpdateTokenExpire(string token)
    {
        (var mr, var mp) = await GameDB.Player.Select(token);
        if (mr != MongoResult.SUCCESS)
            return Error1(mr);
        if (null == mp)
            return Error2(ErrorCode.TOKEN_NOPLAYER);

        var now = GTime.Now();
        if (mp.tokenexpire <= now)
            return Error2(ErrorCode.TOKEN_EXPIRED);

        var tokenexpire = now.AddHours(1);
        mr = await GameDB.Player.Update(mp.id, u => u.Set(p => p.tokenexpire, tokenexpire));
        if (mr != MongoResult.SUCCESS)
            return Error1(mr);

        return (ModuleError.SUCCESS, mp);

        static (ModuleError me, MongoGamePlayer1 mp) Error1(MongoResult mr, [CallerFilePath] string filepath = "", [CallerLineNumber] int line = 0) => (mr.ToModuleError(filepath, line), default);
        static (ModuleError me, MongoGamePlayer1 mp) Error2(ErrorCode ec, [CallerFilePath] string filepath = "", [CallerLineNumber] int line = 0) => (ec.ToModuleError(filepath, line), default);
    }

    public static async Task<ModuleError> Import_Gain(MongoGamePlayer1 mp, List<(CurrencyType type, int amount)> gain)
    {
        var values = gain.GroupBy(g => g.type)
            .Select(x => (type: x.Key, amount: x.Sum(y => y.amount)))
            .ToList();

        if (values.Count <= 0)
            return ModuleError.SUCCESS;

        if (values.Any(x => x.amount <= 0))
            throw new SystemException($"{mp.id} {GJson.SerializeObject(values)}");

        var mr = await GameDB.Player.Update(mp.id, u => BuildQuery_Incr_Currency(u, values));
        if (mr != MongoResult.SUCCESS)
            return Error1(mr);

        foreach (var x in values)
        {
            mp.currency.TryGetValue(x.type, out var old);
            mp.currency[x.type] = old + x.amount;
        }

        return ModuleError.SUCCESS;

        static UpdateDefinition<MongoGamePlayer1> BuildQuery_Incr_Currency(UpdateDefinitionBuilder<MongoGamePlayer1> u, List<(CurrencyType type, int amount)> values)
        {
            return u.Combine(values.Select(x =>
                Builders<MongoGamePlayer1>.Update.Inc(p => p.currency[x.type], x.amount)
            ));
        }

        static ModuleError Error1(MongoResult mr, [CallerFilePath] string filepath = "", [CallerLineNumber] int line = 0) => mr.ToModuleError(filepath, line);
    }
}