using System;
using System.Collections.Generic;
using MongoDB.Driver;
using PNUnity.Share;

namespace PNShare.DB;

public partial class GameDB
{
    public static Player1Collection Player { get; set; } = new Player1Collection();

    public class Player1Collection : CollectionBase<MongoGamePlayer1, long>
    {
        private string _hint_token;
        private string _hint_nickname;
        private string _hint_logintime;

        protected override void InjectionInit()
        {
            _hint_token = _col.Indexes.CreateOne(new CreateIndexModel<MongoGamePlayer1>(
                Builders<MongoGamePlayer1>.IndexKeys.Ascending(p => p.token),
                new CreateIndexOptions() { Unique = true }
            ));
            var ignorecase = new Collation("en", strength: CollationStrength.Secondary);
            _hint_nickname = _col.Indexes.CreateOne(new CreateIndexModel<MongoGamePlayer1>(
                Builders<MongoGamePlayer1>.IndexKeys.Ascending(p => p.nickname),
                new CreateIndexOptions() { Unique = true, Collation = ignorecase }
            ));
            _hint_logintime = _col.Indexes.CreateOne(new CreateIndexModel<MongoGamePlayer1>(
                Builders<MongoGamePlayer1>.IndexKeys.Descending(p => p.logintime),
                new CreateIndexOptions() { }
            ));
        }

        public MongoGamePlayer1 Create(
            long playerid,
            long accountid,
            DateTime create,
            string token,
            int gamech,
            string nickname,
            DateTime logintime
        ) => new MongoGamePlayer1()
        {
            id = playerid,
            accountid = accountid,
            create = create,
            token = token,
            gamech = gamech,
            nickname = nickname,
            currency = new Dictionary<CurrencyType, long>(),
            logintime = logintime,
        };
    }
}