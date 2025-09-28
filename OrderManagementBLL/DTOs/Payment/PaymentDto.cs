namespace OrderManagementBLL.DTOs.Payment;

public class PaymentDto
{
    public long PaymentId { get; set; }
    public long OrderId { get; set; }
    public DateTime PaymentDate { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; }
    public string PaymentStatus { get; set; }
    
    // Audit fields
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    
    // Concurrency control
    public byte[] RowVer { get; set; }
    
    // Extended information (можна завантажувати через join)
    public string OrderNumber { get; set; }
    public string CustomerName { get; set; }
    public decimal OrderTotalAmount { get; set; }
    
    // Computed properties
    public string FormattedAmount => Amount.ToString("C");
    public string FormattedPaymentDate => PaymentDate.ToString("dd.MM.yyyy HH:mm");
    public bool IsSuccessful => PaymentStatus?.ToLower() == "completed" || PaymentStatus?.ToLower() == "success";
    public bool IsPending => PaymentStatus?.ToLower() == "pending" || PaymentStatus?.ToLower() == "processing";
    public bool IsFailed => PaymentStatus?.ToLower() == "failed" || PaymentStatus?.ToLower() == "declined";
    public bool IsRefunded => PaymentStatus?.ToLower() == "refunded" || PaymentStatus?.ToLower() == "cancelled";
    public bool CanBeRefunded => IsSuccessful && !IsRefunded;
    public bool IsPartialPayment => Amount < OrderTotalAmount;
    public bool IsOverpayment => Amount > OrderTotalAmount;
    public decimal PaymentDifference => Amount - OrderTotalAmount;
    public int DaysFromPayment => (DateTime.Now - PaymentDate).Days;
    
}