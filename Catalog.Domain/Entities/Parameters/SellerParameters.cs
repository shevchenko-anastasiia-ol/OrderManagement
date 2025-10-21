namespace Catalog.Domain.Entities.Parameters;

public class SellerParameters : QueryStringParameters
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? CursorId { get; set; }
    public string? SearchText { get; set; }
}