using System.ComponentModel.DataAnnotations;

namespace WarehouseBLL.DTOs.Inventory;

public class InventoryAdjustDto
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Invalid inventory ID")]
    public int Id { get; set; }
    [Required]
    [Range(-1000000, 1000000, ErrorMessage = "Quantity change must be between -1,000,000 and 1,000,000")]
    public int QuantityChange { get; set; }
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "UpdatedBy must be a valid user ID")]
    public int UpdatedBy { get; set; }
}