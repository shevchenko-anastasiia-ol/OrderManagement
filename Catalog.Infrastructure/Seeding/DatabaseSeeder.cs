using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Catalog.Domain.Entities;
using Catalog.Domain.ValueObjects;
using Catalog.Infrastructure.Data;

namespace Catalog.Infrastructure.Seeding;

public class DatabaseSeeder : IDataSeeder
{
    private readonly MongoDbContext _context;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(MongoDbContext context, ILogger<DatabaseSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting database seeding...");
        
        await SeedCategoriesAsync(cancellationToken);
        await SeedSellersAsync(cancellationToken);
        await SeedProductsAsync(cancellationToken);
        await SeedReviewsAsync(cancellationToken);
        
        _logger.LogInformation("Database seeding completed.");
    }

    private string GenerateId() => MongoDB.Bson.ObjectId.GenerateNewId().ToString();

    private async Task SeedCategoriesAsync(CancellationToken cancellationToken)
    {
        var categories = new List<Category>
        {
            new Category("Electronics", "system"),
            new Category("Clothing", "system"),
            new Category("Books", "system"),
            new Category("Home & Garden", "system"),
            new Category("Sports & Outdoors", "system")
        };

        foreach (var category in categories)
        {
            var exists = await _context.Categories
                .Find(c => c.Name == category.Name && !c.IsDeleted)
                .AnyAsync(cancellationToken);

            if (!exists)
            {
                await _context.Categories.InsertOneAsync(category, cancellationToken: cancellationToken);
                _logger.LogInformation("Inserted category: {Name}", category.Name);
            }
        }
    }

    private async Task SeedSellersAsync(CancellationToken cancellationToken)
    {
        var sellers = new List<Seller>
        {
            new Seller("TechStore Ukraine", new Email("techstore@example.com"), new Phone("+380501234567"), "system"),
            new Seller("Fashion Boutique", new Email("fashion@example.com"), new Phone("+380502345678"), "system"),
            new Seller("Book World", new Email("bookworld@example.com"), new Phone("+380503456789"), "system"),
            new Seller("Home Comfort", new Email("homecomfort@example.com"), new Phone("+380504567890"), "system"),
            new Seller("Sport Zone", new Email("sportzone@example.com"), new Phone("+380505678901"), "system")
        };

        foreach (var seller in sellers)
        {
            var exists = await _context.Sellers
                .Find(s => s.Email.Value == seller.Email.Value && !s.IsDeleted)
                .AnyAsync(cancellationToken);

            if (!exists)
            {
                await _context.Sellers.InsertOneAsync(seller, cancellationToken: cancellationToken);
                _logger.LogInformation("Inserted seller: {Name}", seller.Name);
            }
        }
    }

    private async Task SeedProductsAsync(CancellationToken cancellationToken)
    {
        // Отримуємо ID категорій та продавців
        var electronicsCategory = await _context.Categories
            .Find(c => c.Name == "Electronics" && !c.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        var clothingCategory = await _context.Categories
            .Find(c => c.Name == "Clothing" && !c.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        var booksCategory = await _context.Categories
            .Find(c => c.Name == "Books" && !c.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        var techStoreSeller = await _context.Sellers
            .Find(s => s.Email.Value == "techstore@example.com" && !s.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        var fashionSeller = await _context.Sellers
            .Find(s => s.Email.Value == "fashion@example.com" && !s.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        var bookSeller = await _context.Sellers
            .Find(s => s.Email.Value == "bookworld@example.com" && !s.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (techStoreSeller == null || fashionSeller == null || bookSeller == null)
        {
            _logger.LogWarning("Sellers not found. Skipping products seeding.");
            return;
        }

        var products = new List<Product>
        {
            new Product("Laptop HP ProBook 450", new Money(25000, "UAH"), techStoreSeller.Id, "system"),
            new Product("Smartphone Samsung Galaxy S23", new Money(32000, "UAH"), techStoreSeller.Id, "system"),
            new Product("Men's Jacket Winter Collection", new Money(3500, "UAH"), fashionSeller.Id, "system"),
            new Product("Women's Dress Summer Style", new Money(2200, "UAH"), fashionSeller.Id, "system"),
            new Product("Clean Code by Robert Martin", new Money(850, "UAH"), bookSeller.Id, "system")
        };

        // Додаємо категорії до продуктів
        if (electronicsCategory != null)
        {
            products[0].AddCategory(electronicsCategory.Id);
            products[1].AddCategory(electronicsCategory.Id);
        }

        if (clothingCategory != null)
        {
            products[2].AddCategory(clothingCategory.Id);
            products[3].AddCategory(clothingCategory.Id);
        }

        if (booksCategory != null)
        {
            products[4].AddCategory(booksCategory.Id);
        }

        // Додаємо деталі продуктів
        products[0].SetProductDetail(new ProductDetail(
            "Потужний ноутбук для бізнесу з процесором Intel Core i5",
            new Dictionary<string, string>
            {
                { "Processor", "Intel Core i5" },
                { "RAM", "16GB" },
                { "Storage", "512GB SSD" },
                { "Display", "15.6 inch Full HD" }
            }
        ));

        products[1].SetProductDetail(new ProductDetail(
            "Флагманський смартфон з потужною камерою та довгим часом роботи",
            new Dictionary<string, string>
            {
                { "Display", "6.1 inch AMOLED" },
                { "Camera", "50MP + 12MP + 10MP" },
                { "Battery", "3900mAh" },
                { "Storage", "256GB" }
            }
        ));

        products[2].SetProductDetail(new ProductDetail(
            "Стильна чоловіча куртка для холодної погоди",
            new Dictionary<string, string>
            {
                { "Material", "Polyester with insulation" },
                { "Sizes", "S, M, L, XL, XXL" },
                { "Color", "Black, Navy, Grey" }
            }
        ));

        products[3].SetProductDetail(new ProductDetail(
            "Легка літня сукня з натуральних тканин",
            new Dictionary<string, string>
            {
                { "Material", "100% Cotton" },
                { "Sizes", "XS, S, M, L" },
                { "Color", "White, Blue, Pink" }
            }
        ));

        products[4].SetProductDetail(new ProductDetail(
            "Класична книга про принципи чистого коду для програмістів",
            new Dictionary<string, string>
            {
                { "Author", "Robert C. Martin" },
                { "Pages", "464" },
                { "Language", "Ukrainian" },
                { "Publisher", "Vivat" }
            }
        ));

        foreach (var product in products)
        {
            var exists = await _context.Products
                .Find(p => p.Name == product.Name && p.SellerId == product.SellerId && !p.IsDeleted)
                .AnyAsync(cancellationToken);

            if (!exists)
            {
                await _context.Products.InsertOneAsync(product, cancellationToken: cancellationToken);
                _logger.LogInformation("Inserted product: {Name}", product.Name);
            }
        }
    }

    private async Task SeedReviewsAsync(CancellationToken cancellationToken)
    {
        var laptop = await _context.Products
            .Find(p => p.Name == "Laptop HP ProBook 450" && !p.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        var smartphone = await _context.Products
            .Find(p => p.Name == "Smartphone Samsung Galaxy S23" && !p.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        var jacket = await _context.Products
            .Find(p => p.Name == "Men's Jacket Winter Collection" && !p.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (laptop == null || smartphone == null || jacket == null)
        {
            _logger.LogWarning("Products not found. Skipping reviews seeding.");
            return;
        }

        var reviews = new List<Review>
        {
            new Review(laptop.Id, "Олексій Коваленко", 5, "Чудовий ноутбук! Швидкий, надійний, ідеально підходить для роботи.", "user1"),
            new Review(laptop.Id, "Марія Петренко", 4, "Хороший ноутбук, але трохи важкий для носіння.", "user2"),
            new Review(smartphone.Id, "Іван Сидоренко", 5, "Найкращий смартфон, який я мав! Камера просто неймовірна.", "user3"),
            new Review(smartphone.Id, "Ольга Шевченко", 5, "Дуже задоволена покупкою. Швидкий, красивий, функціональний.", "user4"),
            new Review(jacket.Id, "Андрій Мельник", 4, "Тепла куртка, добра якість пошиття. Рекомендую!", "user5")
        };

        // Додаємо відповіді на деякі відгуки
        reviews[0].AddReply("TechStore Support", "Дякуємо за відгук! Раді, що вам сподобався наш продукт.");
        reviews[2].AddReply("TechStore Support", "Вдячні за високу оцінку! Завжди раді допомогти.");

        foreach (var review in reviews)
        {
            var exists = await _context.Reviews
                .Find(r => r.ProductId == review.ProductId && r.Author == review.Author && !r.IsDeleted)
                .AnyAsync(cancellationToken);

            if (!exists)
            {
                await _context.Reviews.InsertOneAsync(review, cancellationToken: cancellationToken);
                _logger.LogInformation("Inserted review for product {ProductId} by {Author}", review.ProductId, review.Author);
            }
        }
    }
}