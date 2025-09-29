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

    public byte[] RowVer { get; set; }

    public decimal LineTotal => Quantity * UnitPrice;
    public string FormattedUnitPrice => UnitPrice.ToString("C");
    public string FormattedLineTotal => LineTotal.ToString("C");

    
}