namespace OrderManagementBLL.DTOs.Payment;

public class PaymentUpdateDto
{
    public string PaymentStatus { get; set; }
    public decimal? Amount { get; set; }
    public string PaymentMethod { get; set; }
    public DateTime? PaymentDate { get; set; }
    public byte[] RowVer { get; set; }
    public string UpdatedBy { get; set; }
}