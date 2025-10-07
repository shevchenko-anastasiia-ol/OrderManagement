namespace MarketplaceDAL.Models;

public class Order
{
    public long OrderId { get; set; }
    public long CustomerId { get; set; }
    public DateTime OrderDate { get; set; }
    public string Status { get; set; }
    public decimal TotalAmount { get; set; }
    
    public DateTime CreatedAt { get; set; }        
    public string CreatedBy { get; set; }          
    public DateTime? UpdatedAt { get; set; }       
    public string UpdatedBy { get; set; }          
    public bool IsDeleted { get; set; }            

    public byte[] RowVer { get; set; }
    
    public List<OrderItem> OrderItems { get; set; }
    
    public List<Payment> Payments { get; set; }
    
    public List<Shipment> Shipments { get; set; }
}