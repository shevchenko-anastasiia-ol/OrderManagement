using Catalog.Domain.Entities;
using Catalog.Infrastructure.Serializers;
using MongoDB.Bson.Serialization;

namespace Catalog.Infrastructure.Mapping;

public static class MongoMappings
{
    public static void Register()
    {
        BsonSerializer.RegisterSerializer(new EmailSerializer());
        BsonSerializer.RegisterSerializer(new PhoneSerializer());
        BsonSerializer.RegisterSerializer(new MoneySerializer());

        if (!BsonClassMap.IsClassMapRegistered(typeof(Product)))
        {
            BsonClassMap.RegisterClassMap<Product>(cm =>
            {
                cm.AutoMap();
                cm.MapIdProperty(p => p.Id);
                cm.MapProperty(p => p.Name);
                cm.MapProperty(p => p.Price);
                cm.MapProperty(p => p.SellerId);
                cm.MapProperty(p => p.Categories);
                cm.MapProperty(p => p.ProductDetail);
                cm.MapProperty(p => p.CreatedAt);
                cm.MapProperty(p => p.CreatedBy);
                cm.MapProperty(p => p.UpdatedAt);
                cm.MapProperty(p => p.UpdatedBy);
                cm.MapProperty(p => p.IsDeleted);
            });
        }

        if (!BsonClassMap.IsClassMapRegistered(typeof(Seller)))
        {
            BsonClassMap.RegisterClassMap<Seller>(cm =>
            {
                cm.AutoMap();
                cm.MapIdProperty(s => s.Id);
                cm.MapProperty(s => s.Name);
                cm.MapProperty(s => s.Email);
                cm.MapProperty(s => s.Phone);
                cm.MapProperty(s => s.CreatedAt);
                cm.MapProperty(s => s.CreatedBy);
                cm.MapProperty(s => s.UpdatedAt);
                cm.MapProperty(s => s.UpdatedBy);
                cm.MapProperty(s => s.IsDeleted);
            });
        }

        if (!BsonClassMap.IsClassMapRegistered(typeof(Review)))
        {
            BsonClassMap.RegisterClassMap<Review>(cm =>
            {
                cm.AutoMap();
                cm.MapIdProperty(r => r.Id);
                cm.MapProperty(r => r.ProductId);
                cm.MapProperty(r => r.Author);
                cm.MapProperty(r => r.Rating);
                cm.MapProperty(r => r.Comment);
                cm.MapProperty(r => r.Replies);
                cm.MapProperty(r => r.CreatedAt);
                cm.MapProperty(r => r.CreatedBy);
                cm.MapProperty(r => r.UpdatedAt);
                cm.MapProperty(r => r.UpdatedBy);
                cm.MapProperty(r => r.IsDeleted);
            });
        }

        if (!BsonClassMap.IsClassMapRegistered(typeof(Category)))
        {
            BsonClassMap.RegisterClassMap<Category>(cm =>
            {
                cm.AutoMap();
                cm.MapIdProperty(c => c.Id);
                cm.MapProperty(c => c.Name);
                cm.MapProperty(c => c.CreatedAt);
                cm.MapProperty(c => c.CreatedBy);
                cm.MapProperty(c => c.UpdatedAt);
                cm.MapProperty(c => c.UpdatedBy);
                cm.MapProperty(c => c.IsDeleted);
            });
        }

        // Допоміжні класи
        if (!BsonClassMap.IsClassMapRegistered(typeof(ProductDetail)))
        {
            BsonClassMap.RegisterClassMap<ProductDetail>(cm =>
            {
                cm.AutoMap();
                cm.MapProperty(pd => pd.Specifications);
                cm.MapProperty(pd => pd.Description);
                cm.MapProperty(pd => pd.Images);
            });
        }

        if (!BsonClassMap.IsClassMapRegistered(typeof(ReviewReply)))
        {
            BsonClassMap.RegisterClassMap<ReviewReply>(cm =>
            {
                cm.AutoMap();
                cm.MapProperty(rr => rr.Author);
                cm.MapProperty(rr => rr.Text);
                cm.MapProperty(rr => rr.CreatedAt);
            });
        }
    }
}