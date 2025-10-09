namespace WarehouseBLL.Helpers;

public class InventoryQueryParams : PaginationParams
{
    public int? WarehouseId { get; set; }
    public int? ProductId { get; set; }
    public int? MinQuantity { get; set; }
    public int? MaxQuantity { get; set; }
    public bool? LowStock { get; set; }
    public int? LowStockThreshold { get; set; } = 10;
    public string? SortBy { get; set; }
    public string SortDirection { get; set; } = "asc";
}