using System.ComponentModel.DataAnnotations;

namespace WarehouseBLL.DTOs.SupplierProduct;

public class SupplierProductDto
{
    public int Id { get; set; }
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Invalid supplier ID")]
    public int SupplierId { get; set; }
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Invalid product ID")]
    public int ProductId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}