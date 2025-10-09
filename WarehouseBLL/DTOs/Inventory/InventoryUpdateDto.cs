using System.ComponentModel.DataAnnotations;

namespace WarehouseBLL.DTOs.Inventory;

public class InventoryUpdateDto
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Invalid inventory ID")]
    public int Id { get; set; }
    [Required]
    [Range(0, 1000000, ErrorMessage = "Quantity must be between 0 and 1,000,000")]
    public int Quantity { get; set; }
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "UpdatedBy must be a valid user ID")]
    public int UpdatedBy { get; set; }
}