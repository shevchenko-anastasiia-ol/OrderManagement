namespace OrderManagementBLL.DTOs.Shipment;

public class ShipmentUpdateDto
{
    public long ShipmentId { get; set; }
    public string TrackingNumber { get; set; }
    public string Carrier { get; set; }
    public string Status { get; set; }
    public DateTime? ShipmentDate { get; set; }
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    public string City { get; set; }
    public string Region { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; }
    public byte[] RowVer { get; set; }
    public string UpdatedBy { get; set; }
    
}