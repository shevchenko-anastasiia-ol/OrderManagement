using System.ComponentModel.DataAnnotations;

namespace WarehouseBLL.DTOs.Supplier;

public class SupplierUpdateDto
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Invalid supplier ID")]
    public int Id { get; set; }
    [Required(ErrorMessage = "Supplier name is required")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 200 characters")]
    public string Name { get; set; } = null!;
    [Required(ErrorMessage = "Country is required")]
    [StringLength(100, ErrorMessage = "Country must not exceed 100 characters")]
    public string Country { get; set; } = null!;
    [StringLength(500, ErrorMessage = "Contact info must not exceed 500 characters")]
    public string? ContactInfo { get; set; }
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "UpdatedBy must be a valid user ID")]
    public int UpdatedBy { get; set; }
}