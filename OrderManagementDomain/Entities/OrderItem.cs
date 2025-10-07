namespace MarketplaceDAL.Models;

public class OrderItem
{
    public long OrderItemId { get; set; }
    public long OrderId { get; set; }
    public long ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    
    public DateTime CreatedAt { get; set; }        
    public string CreatedBy { get; set; }          
    public DateTime? UpdatedAt { get; set; }       
    public string UpdatedBy { get; set; }          
    public bool IsDeleted { get; set; }            

    public byte[] RowVer { get; set; }
}