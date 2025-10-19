using Catalog.Domain.Common;
using Catalog.Domain.Exceptions;
using MongoDB.Bson.Serialization.Attributes;

namespace Catalog.Domain.Entities;

[BsonCollection("Categories")]
public class Category : BaseEntity
{
    [BsonElement("name")]
    public string Name { get; private set; } = string.Empty;

    private Category() { }

    public Category(string name, string userId)
    {
        SetName(name);
        MarkAsCreated(userId);
    }

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Category name cannot be empty.");
        if (name.Length > 100)
            throw new DomainException("Category name cannot exceed 100 characters");
        Name = name.Trim();
    }

    public void Update(string name, string userId)
    {
        SetName(name);
        MarkAsUpdated(userId);
    }
}