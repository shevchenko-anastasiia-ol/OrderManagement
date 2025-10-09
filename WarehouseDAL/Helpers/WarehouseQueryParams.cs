namespace WarehouseBLL.Helpers;

public class WarehouseQueryParams : PaginationParams
{
    public string? SearchTerm { get; set; }
    public int? MinCapacity { get; set; }
    public int? MaxCapacity { get; set; }
    public string? SortBy { get; set; }
    public string SortDirection { get; set; } = "asc";
}