namespace MarketplaceDAL.Models;

public class Shipment
{
    public long ShipmentId { get; set; }
    public long OrderId { get; set; }
    public DateTime ShipmentDate { get; set; }
    public string TrackingNumber { get; set; }
    public string Carrier { get; set; }
    public string Status { get; set; }
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    public string City { get; set; }
    public string Region { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; }
    
    public DateTime CreatedAt { get; set; }      
    public string CreatedBy { get; set; }         
    public DateTime? UpdatedAt { get; set; }       
    public string UpdatedBy { get; set; }         
    public bool IsDeleted { get; set; }            

    public byte[] RowVer { get; set; }
}