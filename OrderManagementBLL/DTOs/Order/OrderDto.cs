using OrderManagementBLL.DTOs.OrderItem;
using OrderManagementBLL.DTOs.Payment;
using OrderManagementBLL.DTOs.Shipment;

namespace OrderManagementBLL.DTOs.Order;

public class OrderDto
{
    public long OrderId { get; set; }
    public long CustomerId { get; set; }
    public DateTime OrderDate { get; set; }
    public string Status { get; set; }
    public decimal TotalAmount { get; set; }
    
    // Audit fields
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    
    // Concurrency control
    public byte[] RowVer { get; set; }
    
    // Navigation properties
    public List<OrderItemDto> OrderItems { get; set; } = new();
    public List<PaymentDto> Payments { get; set; } = new();
    public List<ShipmentDto> Shipments { get; set; } = new();
    
    // Computed properties
    public int TotalItems => OrderItems?.Sum(x => x.Quantity) ?? 0;
    public bool HasPayments => Payments?.Any() == true;
    public bool HasShipments => Shipments?.Any() == true;
    
}