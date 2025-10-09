namespace WarehouseBLL.Helpers;

public class ProductQueryParams : PaginationParams
{
    public string? SearchTerm { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? Sku { get; set; }
    public string? SortBy { get; set; }
    public string SortDirection { get; set; } = "asc";
}