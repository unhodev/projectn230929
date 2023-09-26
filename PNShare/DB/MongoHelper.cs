using System;
using MongoDB.Driver;
using PNShare.Global;

namespace PNShare.DB;

public static class MongoHelper
{
    public static MongoClient GetClient(DbOptions options)
    {
        var connectionstring = options.IsAtlas
            ? $"mongodb+srv://{options.Username}:{options.Password}@{options.Address}/{options.DBName}?retryWrites=true&w=majority"
            : $"mongodb://{options.Username}:{options.Password}@{options.Address}/{options.DBName}?authSource=admin";

        var settings = MongoClientSettings.FromConnectionString(connectionstring);
        if (options.TimeoutMs > 0)
            settings.ServerSelectionTimeout = TimeSpan.FromMilliseconds(options.TimeoutMs);
        if (options.MaxPoolSize > 0)
            settings.MaxConnectionPoolSize = options.MaxPoolSize;

        return new MongoClient(settings);
    }
}