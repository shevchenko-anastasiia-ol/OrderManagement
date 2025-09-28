using OrderManagementBLL.DTOs.OrderItem;

namespace OrderManagementBLL.DTOs.Order;

public class OrderCreateDto
{
    public int CustomerId { get; set; }
    public DateTime OrderDate { get; set; }
    public string Status { get; set; }
    public decimal TotalAmount { get; set; }
    public List<OrderItemCreateDto> OrderItems { get; set; }
    public string IdempotencyToken { get; set; }  
}