using System.ComponentModel.DataAnnotations;

namespace WarehouseBLL.DTOs.Inventory;

public class InventoryCreateDto
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Invalid warehouse ID")]
    public int WarehouseId { get; set; }
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Invalid product ID")]
    public int ProductId { get; set; }
    [Required]
    [Range(0, 1000000, ErrorMessage = "Quantity must be between 0 and 1,000,000")]
    public int Quantity { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "CreatedBy must be a valid user ID")]
    public int CreatedBy { get; set; }
}