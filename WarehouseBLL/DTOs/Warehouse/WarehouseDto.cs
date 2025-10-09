using System.ComponentModel.DataAnnotations;

namespace WarehouseBLL.DTOs.Warehouse;

public class WarehouseDto
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Warehouse name is required")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 200 characters")]
    public string Name { get; set; } = null!;
    [Range(1, 1000000, ErrorMessage = "Capacity must be between 1 and 1,000,000")]
    public int Capacity { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}