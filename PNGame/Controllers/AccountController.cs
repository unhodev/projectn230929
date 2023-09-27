using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using PNShare.DB;
using PNShare.Global;
using PNUnity.Share;

namespace PNGame.Controllers;

public class AccountController : PNGameController
{
    public async Task<IActionResult> Login(string idtoken)
    {
        if (string.IsNullOrEmpty(idtoken))
            return Error(ErrorCode.FAILED);

        (var mr, var ma) = await GameDB.Account.Select(idtoken);
        if (mr != MongoResult.SUCCESS)
            return Error(mr);

        var onnewaccount = null == ma;
        if (onnewaccount)
        {
            (mr, ma) = await CreateAccount(idtoken);
            if (mr != MongoResult.SUCCESS)
                return Error(mr);
        }

        string token;
        var onnewpalyer = ma.lastplayerid <= 0;
        if (onnewpalyer)
        {
            (mr, var mp) = await CreatePlayer(ma);
            if (mr != MongoResult.SUCCESS)
                return Error(mr);
            token = mp.token;
        }
        else
        {
            (mr, token) = await RenewToken(ma.lastplayerid);
            if (mr != MongoResult.SUCCESS)
                return Error(mr);
        }

        return Success(new
        {
            result = (int)ErrorCode.SUCCESS,
            token,
        });

        static async Task<(MongoResult mr, string token)> RenewToken(long playerid)
        {
            MongoResult mr = default;
            string token = default;

            const int MAX_RETRY_CNT = 100;
            for (var i = 0; i < MAX_RETRY_CNT; i++)
            {
                token = Guid.NewGuid().ToString();
                mr = await GameDB.Player.Update(playerid, u => u
                    .Set(p => p.token, token)
                    .Set(p => p.logintime, GTime.Now())
                );
                if (mr == MongoResult.DUPLICATE_ID)
                    continue;
                if (mr != MongoResult.SUCCESS)
                    return (mr, default);
                break;
            }

            return (mr, token);
        }

        static async Task<(MongoResult mr, MongoGamePlayer1 mp)> CreatePlayer(MongoGameAccount ma)
        {
            MongoResult mr = default;
            MongoGamePlayer1 mp = default;

            const int MAX_RETRY_CNT = 100;
            for (var i = 0; i < MAX_RETRY_CNT; i++)
            {
                var token = Guid.NewGuid().ToString();
                var nickname = $"User{token[..8]}";
                mp = GameDB.Player.Create(Random.Shared.NextInt64(), ma.id, GTime.Now(), token, default, nickname, GTime.Now());

                mr = await GameDB.Player.Insert(mp);
                if (mr == MongoResult.DUPLICATE_ID)
                    continue;
                if (mr != MongoResult.SUCCESS)
                    return (mr, default);

                mr = await GameDB.Account.Update(ma.id, u => u.Set(p => p.lastplayerid, mp.id));
                if (mr != MongoResult.SUCCESS)
                    return (mr, default);
                break;
            }

            return (mr, mp);
        }

        static async Task<(MongoResult mr, MongoGameAccount ma)> CreateAccount(string idtoken)
        {
            MongoResult mr = default;
            MongoGameAccount ma = default;

            const int MAX_RETRY_CNT = 100;
            for (var i = 0; i < MAX_RETRY_CNT; i++)
            {
                ma = GameDB.Account.Create(Random.Shared.NextInt64(), idtoken, GTime.Now());

                mr = await GameDB.Account.Insert(ma);
                if (mr == MongoResult.DUPLICATE_ID)
                    continue;
                if (mr != MongoResult.SUCCESS)
                    return (mr, default);
                break;
            }

            return (mr, ma);
        }
    }
}