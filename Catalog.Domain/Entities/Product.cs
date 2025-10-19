using Catalog.Domain.Common;
using Catalog.Domain.Exceptions;
using Catalog.Domain.ValueObjects;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;



namespace Catalog.Domain.Entities;

[BsonCollection("Products")]
public class Product : BaseEntity
{
    [BsonElement("name")]
    public string Name { get; private set; } = string.Empty;

    [BsonElement("price")]
    public Money Price { get; private set; }

    [BsonElement("sellerId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string SellerId { get; private set; } = string.Empty;

    [BsonElement("categories")]
    [BsonRepresentation(BsonType.ObjectId)]
    public List<string> Categories { get; private set; } = new();

    [BsonElement("productDetail")]
    [BsonIgnoreIfNull]
    public ProductDetail? ProductDetail { get; private set; }

    private Product() { Price = new Money(0); }

    public Product(string name, Money price, string sellerId, string userId)
    {
        SetName(name);
        SetPrice(price);
        SetSeller(sellerId);
        MarkAsCreated(userId);
    }

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Product name cannot be empty.");
        if (name.Length > 200)
            throw new DomainException("Product name cannot exceed 200 characters");
        Name = name.Trim();
    }

    public void SetPrice(Money price)
    {
        if (price.Amount <= 0)
            throw new DomainException("Product price must be greater than zero");
        Price = price;
    }

    public void SetSeller(string sellerId)
    {
        if (!ObjectId.TryParse(sellerId, out _))
            throw new DomainException("Invalid seller ID");
        SellerId = sellerId;
    }

    public void AddCategory(string categoryId)
    {
        if (!ObjectId.TryParse(categoryId, out _))
            throw new DomainException("Invalid category ID");
        if (!Categories.Contains(categoryId))
            Categories.Add(categoryId);
    }

    public void RemoveCategory(string categoryId) => Categories.Remove(categoryId);

    public void SetProductDetail(ProductDetail detail)
    {
        ProductDetail = detail ?? throw new ArgumentNullException(nameof(detail));
    }

    public void Update(string name, Money price, string userId)
    {
        SetName(name);
        SetPrice(price);
        MarkAsUpdated(userId);
    }
}