namespace MarketplaceDAL.Models;

public class Product
{
    public long ProductId { get; set; }
    public string ProductName { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    
    public DateTime CreatedAt { get; set; }        
    public string CreatedBy { get; set; }         
    public DateTime? UpdatedAt { get; set; }       
    public string UpdatedBy { get; set; }          
    public bool IsDeleted { get; set; }
    
    public byte[] RowVer { get; set; }
    
    public List<OrderItem> OrderItems { get; set; }
}