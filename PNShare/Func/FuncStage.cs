using System.Collections.Generic;
using PNShare.DB;
using PNUnity.Share;
using PNUnity.Share.Helper;

namespace PNShare.Func;

public static class FuncStage
{
    public static SStageDatas ToSStageDatas(this MongoGameStage1 mstage) => new SStageDatas()
    {
        randomseed = mstage.randomseed,
        killcount = mstage.killcount,
    };

    public static List<(CurrencyType, int)> Kill_Rewards(int seed, int lseq, int rseq)
    {
        var list = new List<(CurrencyType, int)>();
        var n = rseq - lseq;
        if (n <= 0)
            return list;

        for (var seq = lseq + 1; seq <= rseq; seq++)
            list.AddRange(SyncHelper.Stage_Kill_Rewards(seed, seq));

        return list;
    }
}