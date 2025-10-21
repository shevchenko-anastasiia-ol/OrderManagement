namespace Catalog.Domain.Entities.Parameters;

public class ProductParameters : QueryStringParameters
{
    public string? Name { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? CategoryId { get; set; }
    public string? SellerId { get; set; }
    public string? CursorId { get; set; }
    public string? SearchText { get; set; }
}