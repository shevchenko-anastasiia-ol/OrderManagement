using Catalog.Domain.Common;
using Catalog.Domain.Exceptions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Catalog.Domain.Entities;

[BsonCollection("Reviews")]
public class Review : BaseEntity
{
    [BsonElement("productId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string ProductId { get; private set; } = string.Empty;

    [BsonElement("author")]
    public string Author { get; private set; } = string.Empty;

    [BsonElement("rating")]
    public int Rating { get; private set; }

    [BsonElement("comment")]
    public string Comment { get; private set; } = string.Empty;

    [BsonElement("replies")]
    public List<ReviewReply> Replies { get; private set; } = new();

    private Review() { }

    public Review(string productId, string author, int rating, string comment, string userId)
    {
        SetProductId(productId);
        SetAuthor(author);
        SetRating(rating);
        SetComment(comment);
        MarkAsCreated(userId);
    }

    public void SetProductId(string productId)
    {
        if (!ObjectId.TryParse(productId, out _))
            throw new DomainException("Invalid product ID");
        ProductId = productId;
    }

    public void SetAuthor(string author)
    {
        if (string.IsNullOrWhiteSpace(author))
            throw new DomainException("Reviewer cannot be empty.");
        Author = author.Trim();
    }

    public void SetRating(int rating)
    {
        if (rating < 1 || rating > 5)
            throw new DomainException("Rating must be from 1 to 5");
        Rating = rating;
    }

    public void SetComment(string comment)
    {
        if (string.IsNullOrWhiteSpace(comment))
            throw new DomainException("Comment cannot be empty.");
        Comment = comment.Trim();
    }

    public void AddReply(string author, string text)
    {
        var reply = new ReviewReply(author, text);
        Replies.Add(reply);
    }

    public void Update(int rating, string comment, string userId)
    {
        SetRating(rating);
        SetComment(comment);
        MarkAsUpdated(userId);
    }
}