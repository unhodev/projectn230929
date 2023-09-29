using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using PNGame.Modules;
using PNShare.DB;
using PNShare.Func;
using PNUnity.Share;
using PNUnity.Share.Helper;

namespace PNGame.Controllers;

public class PlayerController : PNGameController
{
    public async Task<string> Enter(string token)
    {
        (var me, var mp) = await GModule.Load_Player_UpdateTokenExpire(token);
        if (me != ModuleError.SUCCESS)
            return Error(me);
        (var mr, var mstage) = await GameDB.Stage.SelectOrInsert(mp.id);
        if (mr != MongoResult.SUCCESS)
            return Error(mr);

        mstage.randomseed = Random.Shared.Next();
        mstage.killcount = 0;
        mr = await GameDB.Stage.Update(mp.id, u => u
            .Set(p => p.randomseed, mstage.randomseed)
            .Set(p => p.killcount, mstage.killcount)
        );

        var player = mp.ToSPlayer();
        var stagedatas = mstage.ToSStageDatas();

        return Success(new
        {
            result = (int)ErrorCode.SUCCESS,
            player,
            stagedatas,
        });
    }

    public async Task<string> Sync(string token, string sync)
    {
        (var me, var mp) = await GModule.Load_Player_UpdateTokenExpire(token);
        if (me != ModuleError.SUCCESS)
            return Error(me);

        var syncdatas = UrlEncodedHelper.Decode(sync);
        (me, _, var gain) = await GModule.Import_Sync(mp, syncdatas);
        if (me != ModuleError.SUCCESS)
            return Error(me);

        return Success(new
        {
            result = (int)ErrorCode.SUCCESS,
            gain,
        });
    }
}