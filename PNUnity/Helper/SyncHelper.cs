using System;
using System.Collections.Generic;

namespace PNUnity.Share.Helper
{
    public static class SyncHelper
    {
        public static IEnumerable<(CurrencyType, int)> Stage_Kill_Rewards(int seed, int seq)
        {
            if (seq <= 0)
                yield break;

            var rn = new Random(seed ^ seq);
            var type = (CurrencyType)rn.Next((int)CurrencyType.Length);
            var amount = rn.Next(1, 100);
            yield return (type, amount);
        }
    }
}