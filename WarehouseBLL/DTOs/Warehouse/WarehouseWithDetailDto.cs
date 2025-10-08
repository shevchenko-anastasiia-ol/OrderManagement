using WarehouseBLL.DTOs.WarehouseDetail;

namespace WarehouseBLL.DTOs.Warehouse;

public class WarehouseWithDetailDto : WarehouseDto
{
    public WarehouseDetailDto? Details { get; set; }
}