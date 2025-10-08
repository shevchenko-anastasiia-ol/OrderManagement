namespace WarehouseBLL.Helpers;

public class SupplierQueryParams : PaginationParams
{
    public string? SearchTerm { get; set; }
    public string? Country { get; set; }
    public string? SortBy { get; set; }
    public string SortDirection { get; set; } = "asc";
}