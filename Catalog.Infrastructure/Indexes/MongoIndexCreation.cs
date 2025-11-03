using Catalog.Domain.Entities;
using Catalog.Infrastructure.Data;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Indexes;

public class MongoIndexCreation : IIndexCreation
{
    private readonly MongoDbContext _context;

    public MongoIndexCreation(MongoDbContext context)
    {
        _context = context;
    }

    public async Task CreateIndexesAsync(CancellationToken cancellationToken = default)
    {
        await CreateProductIndexesAsync(cancellationToken);
        await CreateSellerIndexesAsync(cancellationToken);
        await CreateReviewIndexesAsync(cancellationToken);
        await CreateCategoryIndexesAsync(cancellationToken);
    }

    private async Task CreateProductIndexesAsync(CancellationToken cancellationToken)
    {
        var collection = _context.Products;

        // Індекс за продавцем
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<Product>(
                Builders<Product>.IndexKeys.Ascending(p => p.SellerId),
                new CreateIndexOptions { Name = "idx_seller_id" }
            ),
            cancellationToken: cancellationToken
        );

        // Індекс за категоріями
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<Product>(
                Builders<Product>.IndexKeys.Ascending(p => p.Categories),
                new CreateIndexOptions { Name = "idx_categories" }
            ),
            cancellationToken: cancellationToken
        );

        // Текстовий індекс для пошуку за назвою
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<Product>(
                Builders<Product>.IndexKeys.Text(p => p.Name),
                new CreateIndexOptions { Name = "idx_name_text" }
            ),
            cancellationToken: cancellationToken
        );

        // Індекс за ціною
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<Product>(
                Builders<Product>.IndexKeys.Ascending("price.amount"),
                new CreateIndexOptions { Name = "idx_price" }
            ),
            cancellationToken: cancellationToken
        );

        // Складний індекс: продавець + ціна
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<Product>(
                Builders<Product>.IndexKeys
                    .Ascending(p => p.SellerId)
                    .Ascending("price.amount"),
                new CreateIndexOptions { Name = "idx_seller_price" }
            ),
            cancellationToken: cancellationToken
        );

        // Індекс за датою створення
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<Product>(
                Builders<Product>.IndexKeys.Descending(p => p.CreatedAt),
                new CreateIndexOptions { Name = "idx_created_at" }
            ),
            cancellationToken: cancellationToken
        );

        // Індекс для м'якого видалення
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<Product>(
                Builders<Product>.IndexKeys.Ascending(p => p.IsDeleted),
                new CreateIndexOptions { Name = "idx_is_deleted" }
            ),
            cancellationToken: cancellationToken
        );
    }

    private async Task CreateSellerIndexesAsync(CancellationToken cancellationToken)
    {
        var collection = _context.Sellers;

        // Унікальний індекс за email
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<Seller>(
                Builders<Seller>.IndexKeys.Ascending("email.value"),
                new CreateIndexOptions 
                { 
                    Name = "idx_email_unique",
                    Unique = true 
                }
            ),
            cancellationToken: cancellationToken
        );

        // Індекс за телефоном
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<Seller>(
                Builders<Seller>.IndexKeys.Ascending("phone.value"),
                new CreateIndexOptions { Name = "idx_phone" }
            ),
            cancellationToken: cancellationToken
        );

        // Текстовий індекс для пошуку за ім'ям
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<Seller>(
                Builders<Seller>.IndexKeys.Text(s => s.Name),
                new CreateIndexOptions { Name = "idx_name_text" }
            ),
            cancellationToken: cancellationToken
        );

        // Індекс за датою створення
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<Seller>(
                Builders<Seller>.IndexKeys.Descending(s => s.CreatedAt),
                new CreateIndexOptions { Name = "idx_created_at" }
            ),
            cancellationToken: cancellationToken
        );

        // Індекс для м'якого видалення
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<Seller>(
                Builders<Seller>.IndexKeys.Ascending(s => s.IsDeleted),
                new CreateIndexOptions { Name = "idx_is_deleted" }
            ),
            cancellationToken: cancellationToken
        );
    }

    private async Task CreateReviewIndexesAsync(CancellationToken cancellationToken)
    {
        var collection = _context.Reviews;

        // Індекс за продуктом
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<Review>(
                Builders<Review>.IndexKeys.Ascending(r => r.ProductId),
                new CreateIndexOptions { Name = "idx_product_id" }
            ),
            cancellationToken: cancellationToken
        );

        // Індекс за автором
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<Review>(
                Builders<Review>.IndexKeys.Ascending(r => r.Author),
                new CreateIndexOptions { Name = "idx_author" }
            ),
            cancellationToken: cancellationToken
        );

        // Унікальний індекс: один відгук від автора на продукт
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<Review>(
                Builders<Review>.IndexKeys
                    .Ascending(r => r.ProductId)
                    .Ascending(r => r.Author),
                new CreateIndexOptions 
                { 
                    Name = "idx_product_author_unique",
                    Unique = true 
                }
            ),
            cancellationToken: cancellationToken
        );

        // Індекс за рейтингом
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<Review>(
                Builders<Review>.IndexKeys.Ascending(r => r.Rating),
                new CreateIndexOptions { Name = "idx_rating" }
            ),
            cancellationToken: cancellationToken
        );

        // Складний індекс: продукт + рейтинг
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<Review>(
                Builders<Review>.IndexKeys
                    .Ascending(r => r.ProductId)
                    .Descending(r => r.Rating),
                new CreateIndexOptions { Name = "idx_product_rating" }
            ),
            cancellationToken: cancellationToken
        );

        // Текстовий індекс для пошуку в коментарях
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<Review>(
                Builders<Review>.IndexKeys.Text(r => r.Comment),
                new CreateIndexOptions { Name = "idx_comment_text" }
            ),
            cancellationToken: cancellationToken
        );

        // Індекс за датою створення
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<Review>(
                Builders<Review>.IndexKeys.Descending(r => r.CreatedAt),
                new CreateIndexOptions { Name = "idx_created_at" }
            ),
            cancellationToken: cancellationToken
        );

        // Індекс для м'якого видалення
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<Review>(
                Builders<Review>.IndexKeys.Ascending(r => r.IsDeleted),
                new CreateIndexOptions { Name = "idx_is_deleted" }
            ),
            cancellationToken: cancellationToken
        );
    }

    private async Task CreateCategoryIndexesAsync(CancellationToken cancellationToken)
    {
        var collection = _context.Categories;

        // Унікальний індекс за назвою
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<Category>(
                Builders<Category>.IndexKeys.Ascending(c => c.Name),
                new CreateIndexOptions 
                { 
                    Name = "idx_name_unique",
                    Unique = true 
                }
            ),
            cancellationToken: cancellationToken
        );

        // Текстовий індекс для пошуку
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<Category>(
                Builders<Category>.IndexKeys.Text(c => c.Name),
                new CreateIndexOptions { Name = "idx_name_text" }
            ),
            cancellationToken: cancellationToken
        );

        // Індекс за датою створення
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<Category>(
                Builders<Category>.IndexKeys.Descending(c => c.CreatedAt),
                new CreateIndexOptions { Name = "idx_created_at" }
            ),
            cancellationToken: cancellationToken
        );

        // Індекс для м'якого видалення
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<Category>(
                Builders<Category>.IndexKeys.Ascending(c => c.IsDeleted),
                new CreateIndexOptions { Name = "idx_is_deleted" }
            ),
            cancellationToken: cancellationToken
        );
    }
}