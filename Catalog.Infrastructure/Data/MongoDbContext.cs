using Catalog.Domain.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson;

namespace Catalog.Infrastructure.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;
    public IMongoDatabase Database => _database;

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        var mongoSettings = MongoClientSettings.FromConnectionString(settings.Value.ConnectionString);
        mongoSettings.MaxConnectionPoolSize = settings.Value.MaxConnectionPoolSize;
        mongoSettings.MinConnectionPoolSize = settings.Value.MinConnectionPoolSize;
        mongoSettings.ConnectTimeout = TimeSpan.FromSeconds(settings.Value.ConnectTimeoutSeconds);
        mongoSettings.SocketTimeout = TimeSpan.FromSeconds(settings.Value.SocketTimeoutSeconds);

        var client = new MongoClient(mongoSettings);
        _database = client.GetDatabase(settings.Value.DatabaseName);
    }

    public IMongoCollection<Category> Categories => _database.GetCollection<Category>("Categories");
    public IMongoCollection<Product> Products => _database.GetCollection<Product>("Products");
    public IMongoCollection<Review> Reviews => _database.GetCollection<Review>("Reviews");
    public IMongoCollection<Seller> Sellers => _database.GetCollection<Seller>("Sellers");
    
    public IClientSessionHandle StartSession() => _database.Client.StartSession();
}