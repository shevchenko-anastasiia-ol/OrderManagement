namespace OrderManagementBLL.DTOs.Customer;

public class CustomerUpdateDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public byte[] RowVer { get; set; }
    public string UpdatedBy { get; set; }
}