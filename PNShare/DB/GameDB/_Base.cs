using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using PNShare.Global;

namespace PNShare.DB;

public static partial class GameDB
{
    private static MongoClient _client;
    private static IMongoDatabase _db;

    public static void Init(DbOptions options)
    {
        var client = MongoHelper.GetClient(options);
        _client = client;
        var db = client.GetDatabase(options.DBName);
        _db = db;
    }

    public static async Task<string> Ping()
    {
        var r = await _db.RunCommandAsync((Command<BsonDocument>)"{ ping: 1 }");
        return r.ToJson();
    }
}