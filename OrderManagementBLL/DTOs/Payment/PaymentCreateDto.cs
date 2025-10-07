namespace OrderManagementBLL.DTOs.Payment;

public class PaymentCreateDto
{
    public long OrderId { get; set; }
    public DateTime PaymentDate { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; }
    public string PaymentStatus { get; set; } = "Pending";
    public string CreatedBy { get; set; }
    
    public string IdempotencyToken { get; set; }  
}