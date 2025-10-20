namespace Catalog.Domain.Entities;

[AttributeUsage(AttributeTargets.Class)]
public class BsonCollectionAttribute : Attribute
{
    public string CollectionName { get; }
    public BsonCollectionAttribute(string collectionName) => CollectionName = collectionName;
}