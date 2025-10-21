namespace Catalog.Domain.Entities.Parameters;

public class CategoryParameters  : QueryStringParameters
{
    public string? Name { get; set; }
    public string? ParentCategoryId { get; set; }
    public string? CursorId { get; set; }
}