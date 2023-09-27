using System;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace PNShare.DB;

public partial class GameDB
{
    public static AccountCollection Account { get; set; } = new AccountCollection();

    public class AccountCollection : CollectionBase<MongoGameAccount, long>
    {
        private string _hint_idkey;

        protected override void InjectionInit()
        {
            _hint_idkey = _col.Indexes.CreateOne(new CreateIndexModel<MongoGameAccount>(
                Builders<MongoGameAccount>.IndexKeys.Ascending(p => p.idkey),
                new CreateIndexOptions { Unique = true })
            );
        }

        public MongoGameAccount Create(long accountid, string idkey, DateTime now) => new MongoGameAccount()
        {
            id = accountid,
            create = now,
            idkey = idkey,
        };

        public async Task<(MongoResult mr, MongoGameAccount maccount)> Select(string idkey)
        {
            var filter = Builders<MongoGameAccount>.Filter.Eq(p => p.idkey, idkey);
            try
            {
                return (MongoResult.SUCCESS, await _col.Find(filter).FirstOrDefaultAsync());
            }
            catch (Exception e)
            {
                return (e.AsMr(), default);
            }
        }
    }
}