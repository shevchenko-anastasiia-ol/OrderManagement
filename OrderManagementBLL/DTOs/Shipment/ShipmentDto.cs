using OrderManagementBLL.DTOs.OrderItem;

namespace OrderManagementBLL.DTOs.Shipment;

public class ShipmentDto
{
    public long ShipmentId { get; set; }
    public long OrderId { get; set; }
    public DateTime ShipmentDate { get; set; }
    public string TrackingNumber { get; set; }
    public string Carrier { get; set; }
    public string Status { get; set; }
    
    // Адреса доставки
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    public string City { get; set; }
    public string Region { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; }
    
    // Audit fields
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    
    // Concurrency control
    public byte[] RowVer { get; set; }
    
    public string OrderNumber { get; set; }
    public string CustomerName { get; set; }
    public string CustomerPhone { get; set; }
    public string CustomerEmail { get; set; }
    public decimal OrderTotalAmount { get; set; }
    public List<OrderItemDto> OrderItems { get; set; } = new();
}