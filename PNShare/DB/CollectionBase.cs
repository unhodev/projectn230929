using System;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace PNShare.DB;

public interface IMongoDocument<T>
{
    public T id { get; set; }
}

public abstract class CollectionBase<TDocument, TId> where TDocument : IMongoDocument<TId>
{
    public delegate UpdateDefinition<TDocument> UpdateFunc1(UpdateDefinitionBuilder<TDocument> u);

    protected IMongoDatabase _db;
    protected IMongoCollection<TDocument> _col;

    public void Init(IMongoDatabase db, string name)
    {
        _db = db;
        _col = db.GetCollection<TDocument>(name);

        InjectionInit();
    }

    protected virtual void InjectionInit()
    {
    }

    public async Task<MongoResult> Insert(TDocument document)
    {
        try
        {
            await _col.InsertOneAsync(document);
            return MongoResult.SUCCESS;
        }
        catch (Exception e)
        {
            return e.AsMr();
        }
    }

    public async Task<(MongoResult mr, TDocument mdoc)> Select(TId documentid)
    {
        var filter = Builders<TDocument>.Filter.Eq(p => p.id, documentid);
        try
        {
            return (MongoResult.SUCCESS, await _col.Find(filter).FirstOrDefaultAsync());
        }
        catch (Exception e)
        {
            return (e.AsMr(), default);
        }
    }

    public async Task<MongoResult> Update(TId documentid, UpdateFunc1 func1)
    {
        var filter = Builders<TDocument>.Filter.Eq(p => p.id, documentid);
        var update = func1(Builders<TDocument>.Update);
        try
        {
            var r = await _col.UpdateOneAsync(filter, update);
            return MongoResult.SUCCESS;
        }
        catch (Exception e)
        {
            return e.AsMr();
        }
    }

    public async Task<(MongoResult mr, int count)> Count()
    {
        var filter = Builders<TDocument>.Filter.Empty;
        try
        {
            return (MongoResult.SUCCESS, (int)await _col.CountDocumentsAsync(filter));
        }
        catch (Exception e)
        {
            return (e.AsMr(), default);
        }
    }
}