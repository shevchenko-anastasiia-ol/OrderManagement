using Catalog.Domain.Exceptions;
using MongoDB.Bson.Serialization.Attributes;

namespace Catalog.Domain.Entities;

[BsonIgnoreExtraElements]
public class ProductDetail
{
    [BsonElement("specifications")]
    public Dictionary<string, string> Specifications { get; private set; } = new();

    [BsonElement("description")]
    public string Description { get; private set; } = string.Empty;

    [BsonElement("images")]
    public List<string> Images { get; private set; } = new();

    private ProductDetail() { }

    public ProductDetail(string description, Dictionary<string, string>? specifications = null)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Product description cannot be empty.");
        Description = description.Trim();
        Specifications = specifications ?? new Dictionary<string, string>();
    }

    public void AddImage(string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            throw new DomainException("Image URL cannot be empty.");
        Images.Add(imageUrl);
    }

    public void AddSpecification(string key, string value)
    {
        if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
            throw new DomainException("The specification cannot contain empty values.");
        Specifications[key] = value;
    }
}