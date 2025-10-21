namespace Catalog.Domain.Entities.Parameters;

public class ReviewParameters : QueryStringParameters
{
    public string? ProductId { get; set; }
    public string? AuthorId { get; set; }
    public string? SearchText { get; set; }
    public string? CursorId { get; set; }
}