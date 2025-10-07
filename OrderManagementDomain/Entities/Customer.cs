namespace MarketplaceDAL.Models;

public class Customer
{
    public long CustomerId { get; set; }
    public string FirstName { get; set; } 
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    
    public byte[] RowVer { get; set; }
    
    public List<Order> Orders { get; set; }
}