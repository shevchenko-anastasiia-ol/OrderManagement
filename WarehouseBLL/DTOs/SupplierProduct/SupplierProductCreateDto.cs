using System.ComponentModel.DataAnnotations;

namespace WarehouseBLL.DTOs.SupplierProduct;

public class SupplierProductCreateDto
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Invalid supplier ID")]
    public int SupplierId { get; set; }
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Invalid product ID")]
    public int ProductId { get; set; }
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "CreatedBy must be a valid user ID")]
    public int CreatedBy { get; set; }
}