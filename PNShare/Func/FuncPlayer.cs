using System;
using PNShare.DB;
using PNUnity.Share;

namespace PNShare.Func;

public static class FuncPlayer
{
    public static SPlayer ToSPlayer(this MongoGamePlayer1 mp) => new SPlayer()
    {
        playerid = mp.id,
        accountid = mp.accountid,
        create = mp.create,
        gamech = mp.gamech,
        nickname = mp.nickname,
        currency = mp.currency,
    };

    public static DateTime NewTokenExpire(DateTime now) => now.AddHours(1);
}