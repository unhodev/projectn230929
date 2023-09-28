using System.Threading.Tasks;
using PNGame.Modules;
using PNShare.Func;
using PNUnity.Share;

namespace PNGame.Controllers;

public class PlayerController : PNGameController
{
    public async Task<string> Enter(string token)
    {
        (var me, var mp) = await GModule.Load_Player_UpdateTokenExpire(token);
        if (me != ModuleError.SUCCESS)
            return Error(me);

        var player = mp.ToSPlayer();

        return Success(new
        {
            result = (int)ErrorCode.SUCCESS,
            player,
        });
    }
}