using System;
using MongoDB.Driver;
using PNShare.Global;

namespace PNShare.DB;

public enum MongoResult
{
    SUCCESS,
    ETC,
    WRONG_AUTH,
    DUPLICATE_ID,
    WRONG_SCHEMA,
    CONNECT_ERROR,
}

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

    public static MongoResult AsMr(this Exception exception)
    {
        var mr = exception switch
        {
            MongoAuthenticationException => MongoResult.WRONG_AUTH,
            MongoWriteException w => w.WriteError.Category == ServerErrorCategory.DuplicateKey ? MongoResult.DUPLICATE_ID : MongoResult.ETC,
            TimeoutException => MongoResult.CONNECT_ERROR,
            FormatException => MongoResult.WRONG_SCHEMA,
            _ => MongoResult.ETC
        };
        if (mr == MongoResult.ETC)
            GLog.Write(exception);

        return mr;
    }
}