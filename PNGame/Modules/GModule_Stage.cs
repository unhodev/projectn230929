using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using PNShare.DB;
using PNShare.Func;
using PNUnity.Share;

namespace PNGame.Modules;

public partial class GModule
{
    public static async Task<(ModuleError me, MongoGameStage1 mstage, List<(CurrencyType type, int amount)> gain)> Import_Sync(MongoGamePlayer1 mp, SSyncDatas syncdatas)
    {
        (var mr, var mstage) = await GameDB.Stage.SelectOrInsert(mp.id);
        if (mr != MongoResult.SUCCESS)
            return Error1(mr);

        var gain = new List<(CurrencyType type, int amount)>();
        if (mstage.killcount <= syncdatas.killcount)
        {
            gain.AddRange(FuncStage.Kill_Rewards(mstage.randomseed, mstage.killcount, syncdatas.killcount));

            mr = await GameDB.Stage.Update(mp.id, u => u.Set(p => p.killcount, syncdatas.killcount));
            if (mr != MongoResult.SUCCESS)
                return Error1(mr);
            mstage.killcount = syncdatas.killcount;

            var me = await Import_Gain(mp, gain);
            if (me != ModuleError.SUCCESS)
                return Error3(me);
        }

        return (ModuleError.SUCCESS, mstage, gain);

        static (ModuleError me, MongoGameStage1 mstage, List<(CurrencyType type, int amount)> gain) Error1(MongoResult mr, [CallerFilePath] string filepath = "", [CallerLineNumber] int line = 0) => (mr.ToModuleError(filepath, line), default, default);
        static (ModuleError me, MongoGameStage1 mstage, List<(CurrencyType type, int amount)> gain) Error2(ErrorCode ec, [CallerFilePath] string filepath = "", [CallerLineNumber] int line = 0) => (ec.ToModuleError(filepath, line), default, default);
        static (ModuleError me, MongoGameStage1 mstage, List<(CurrencyType type, int amount)> gain) Error3(ModuleError me) => (me, default, default);
    }
}