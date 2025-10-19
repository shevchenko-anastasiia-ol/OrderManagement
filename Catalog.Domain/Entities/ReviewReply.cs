using Catalog.Domain.Exceptions;
using MongoDB.Bson.Serialization.Attributes;

namespace Catalog.Domain.Entities;

[BsonIgnoreExtraElements]
public class ReviewReply
{
    [BsonElement("author")]
    public string Author { get; private set; } = string.Empty;

    [BsonElement("text")]
    public string Text { get; private set; } = string.Empty;

    [BsonElement("createdAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime CreatedAt { get; private set; }

    private ReviewReply() { }

    public ReviewReply(string author, string text)
    {
        if (string.IsNullOrWhiteSpace(author))
            throw new DomainException("Replyer cannot be empty");
        if (string.IsNullOrWhiteSpace(text))
            throw new DomainException("The response text cannot be empty.");
        Author = author.Trim();
        Text = text.Trim();
        CreatedAt = DateTime.UtcNow;
    }
}