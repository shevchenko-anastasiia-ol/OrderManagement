namespace OrderManagementBLL.DTOs.Customer;

public class CustomerCreateDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string CreatedBy { get; set; }
}