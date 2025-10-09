using System.ComponentModel.DataAnnotations;

namespace WarehouseBLL.DTOs.Product;

public class ProductCreateDto
{
    [Required(ErrorMessage = "Product name is required")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 200 characters")]
    public string Name { get; set; } = null!;
    [Required(ErrorMessage = "SKU is required")]
    [StringLength(100, ErrorMessage = "SKU must not exceed 100 characters")]
    [RegularExpression(@"^[A-Za-z0-9-_]+$", ErrorMessage = "SKU can only contain alphanumeric characters, hyphens, and underscores")]
    public string SKU { get; set; } = null!;
    [Required]
    [Range(0.01, 1000000, ErrorMessage = "Price must be between 0.01 and 1,000,000")]
    [DataType(DataType.Currency)]
    public decimal Price { get; set; }
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "CreatedBy must be a valid user ID")]
    public int CreatedBy { get; set; }
}