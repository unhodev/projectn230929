using System.Threading.Tasks;

namespace PNShare.DB;

public partial class GameDB
{
    public static Stage1Collection Stage { get; set; } = new Stage1Collection();

    public class Stage1Collection : CollectionBase<MongoGameStage1, long>
    {
        public MongoGameStage1 Create(long playerid) => new MongoGameStage1()
        {
            id = playerid,
            randomseed = 0,
            killcount = 0,
        };

        public async Task<(MongoResult mr, MongoGameStage1 mstage)> SelectOrInsert(long playerid)
        {
            (var mr, var mstage) = await Select(playerid);
            if (mr != MongoResult.SUCCESS)
                return (mr, default);
            if (null != mstage)
                return (mr, mstage);

            mstage = Create(playerid);
            mr = await Insert(mstage);

            return (mr, mstage);
        }
    }
}