using OrderManagementBLL.DTOs.Order;

namespace OrderManagementBLL.DTOs.Customer;

public class CustomerDto
{
    public long CustomerId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    
    // Audit fields
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    
    // Concurrency control
    public byte[] RowVer { get; set; }
    
    // Navigation properties
    public List<OrderDto> Orders { get; set; } = new();
    
    // Computed properties
    public string FullName => $"{FirstName} {LastName}".Trim();
    public string DisplayName => !string.IsNullOrEmpty(FullName) ? FullName : Email;
    public int TotalOrders => Orders?.Count ?? 0;
    public decimal TotalSpent => Orders?.Sum(o => o.TotalAmount) ?? 0;
    public DateTime? LastOrderDate => Orders?.OrderByDescending(o => o.OrderDate).FirstOrDefault()?.OrderDate;
    public string CustomerSince => CreatedAt.ToString("MMMM yyyy");
    public bool IsActive => !IsDeleted && (LastOrderDate?.AddMonths(6) >= DateTime.Now || TotalOrders == 0);

}