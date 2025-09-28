namespace OrderManagementBLL.DTOs.OrderItem;

public class OrderItemDto
{
    public long OrderItemId { get; set; }
    public long OrderId { get; set; }
    public long ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    
    // Audit fields
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    
    // Concurrency control
    public byte[] RowVer { get; set; }
    
    // Extended product information (можна завантажувати через join або включення)
    public string ProductName { get; set; }
    public string ProductSku { get; set; }
    public string ProductCategory { get; set; }
    public decimal ProductCurrentPrice { get; set; }
    public bool ProductIsAvailable { get; set; }
    
    // Computed properties
    public decimal LineTotal => Quantity * UnitPrice;
    public string FormattedUnitPrice => UnitPrice.ToString("C");
    public string FormattedLineTotal => LineTotal.ToString("C");
    public string ProductDisplayName => !string.IsNullOrEmpty(ProductSku) 
        ? $"{ProductName} ({ProductSku})" 
        : ProductName;
    
}