namespace OrderManagementBLL.DTOs.OrderItem;

public class OrderItemCreateDto
{
    public long ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string CreatedBy { get; set; }
    public string IdempotencyToken { get; set; }  
}