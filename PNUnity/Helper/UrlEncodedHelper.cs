using System.Linq;

namespace PNUnity.Share.Helper;

public static class UrlEncodedHelper
{
    public static string Encode(this SSyncDatas datas) => string.Join(';', datas.killcount);

    public static SSyncDatas Decode(string sync)
    {
        var args = sync.Split(';').ToList();
        var datas = new SSyncDatas();
        datas.killcount = int.TryParse(args[0], out var killcount) ? killcount : default;
        return datas;
    }
}