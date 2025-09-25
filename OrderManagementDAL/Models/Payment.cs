namespace MarketplaceDAL.Models;

public class Payment
{
    public long PaymentId { get; set; }
    public long OrderId { get; set; }
    public DateTime PaymentDate { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; }
    public string PaymentStatus { get; set; }
    
    public DateTime CreatedAt { get; set; }        // datetime2
    public string CreatedBy { get; set; }          // nvarchar(50)
    public DateTime? UpdatedAt { get; set; }       // datetime2 (може бути null)
    public string UpdatedBy { get; set; }          // nvarchar(50)
    public bool IsDeleted { get; set; }            // bit

    public byte[] RowVer { get; set; }
    
}