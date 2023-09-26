using MongoDB.Driver;
using PNShare.Global;

namespace PNShare.DB;

public static class GameDB
{
    private static MongoClient _client;
    private static IMongoDatabase _db;

    public static void Init(DbOptions options)
    {
        var client = MongoHelper.GetClient(options);
        _client = client;
        _db = client.GetDatabase(options.DBName);
    }
}