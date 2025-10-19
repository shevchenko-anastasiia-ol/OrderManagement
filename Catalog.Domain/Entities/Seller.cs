using Catalog.Domain.Common;
using Catalog.Domain.Exceptions;
using Catalog.Domain.ValueObjects;
using MongoDB.Bson.Serialization.Attributes;

namespace Catalog.Domain.Entities;

[BsonCollection("Sellers")]
public class Seller : BaseEntity
{
    [BsonElement("name")]
    public string Name { get; private set; } = string.Empty;

    [BsonElement("email")]
    public Email Email { get; private set; }

    [BsonElement("phone")]
    public Phone Phone { get; private set; }

    private Seller() 
    { 
        Email = new Email("default@example.com");
        Phone = new Phone("+380000000000");
    }

    public Seller(string name, Email email, Phone phone, string userId)
    {
        SetName(name);
        SetEmail(email);
        SetPhone(phone);
        MarkAsCreated(userId);
    }

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Seller name cannot be empty.");
        if (name.Length > 150)
            throw new DomainException("Seller name cannot exceed 150 characters.");
        Name = name.Trim();
    }

    public void SetEmail(Email email) => Email = email ?? throw new ArgumentNullException(nameof(email));
    public void SetPhone(Phone phone) => Phone = phone ?? throw new ArgumentNullException(nameof(phone));

    public void Update(string name, Email email, Phone phone, string userId)
    {
        SetName(name);
        SetEmail(email);
        SetPhone(phone);
        MarkAsUpdated(userId);
    }
}