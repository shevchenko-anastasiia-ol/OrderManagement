namespace OrderManagementBLL.DTOs.Product;

public class ProductUpdateDto
{
    public long ProductId { get; set; }
    public string ProductName { get; set; }
    public string Description { get; set; }
    public decimal? Price { get; set; }
    public int? StockQuantity { get; set; }
    public byte[] RowVer { get; set; }
    public string UpdatedBy { get; set; }
}