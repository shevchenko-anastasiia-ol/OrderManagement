using OrderManagementBLL.DTOs.OrderItem;

namespace OrderManagementBLL.DTOs.Order;

public class OrderUpdateDto
{
    public  long OrderId { get; set; }
    public string Status { get; set; }
    public decimal? TotalAmount { get; set; }
    public List<OrderItemUpdateDto> OrderItems { get; set; }
    public byte[] RowVer { get; set; }
    public string UpdatedBy { get; set; }
}