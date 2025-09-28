namespace OrderManagementBLL.DTOs.OrderItem;

public class OrderItemUpdateDto
{
    public int? Quantity { get; set; }
    public decimal? UnitPrice { get; set; }
    public byte[] RowVer { get; set; }
    public string UpdatedBy { get; set; }
}