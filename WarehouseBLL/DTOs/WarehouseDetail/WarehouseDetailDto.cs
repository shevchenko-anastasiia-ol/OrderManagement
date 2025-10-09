using System.ComponentModel.DataAnnotations;

namespace WarehouseBLL.DTOs.WarehouseDetail;

public class WarehouseDetailDto
{
    public int Id { get; set; }
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Invalid warehouse ID")]
    public int WarehouseId { get; set; }
    [StringLength(500, ErrorMessage = "Address must not exceed 500 characters")]
    public string? Address { get; set; }
    [StringLength(200, ErrorMessage = "Manager name must not exceed 200 characters")]
    public string? Manager { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}