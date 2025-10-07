namespace OrderManagementBLL.DTOs.Shipment;

public class ShipmentCreateDto
{
    public long OrderId { get; set; }
    public DateTime ShipmentDate { get; set; }
    public string Carrier { get; set; }
    public string Status { get; set; } = "Created";
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    public string City { get; set; }
    public string Region { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; } = "Ukraine";
    public string CreatedBy { get; set; }
    public string IdempotencyToken { get; set; }  
    
}