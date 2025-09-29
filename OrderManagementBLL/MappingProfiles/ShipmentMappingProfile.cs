using AutoMapper;
using MarketplaceDAL.Models;
using OrderManagementBLL.DTOs.Shipment;

namespace OrderManagementBLL.MappingProfiles;

public class ShipmentMappingProfile : Profile
{
    public ShipmentMappingProfile()
    {
        CreateMap<Shipment, ShipmentDto>()
            .ForMember(dest => dest.OrderNumber, opt => opt.Ignore())
            .ForMember(dest => dest.CustomerName, opt => opt.Ignore())
            .ForMember(dest => dest.CustomerPhone, opt => opt.Ignore())
            .ForMember(dest => dest.CustomerEmail, opt => opt.Ignore())
            .ForMember(dest => dest.OrderTotalAmount, opt => opt.Ignore())
            .ForMember(dest => dest.OrderItems, opt => opt.Ignore());
        CreateMap<ShipmentCreateDto, Shipment>()
            .ForMember(dest => dest.ShipmentId, opt => opt.Ignore())
            .ForMember(dest => dest.TrackingNumber, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.RowVer, opt => opt.Ignore());
        CreateMap<ShipmentUpdateDto, Shipment>()
            .ForMember(dest => dest.ShipmentId, opt => opt.Ignore())
            .ForMember(dest => dest.OrderId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
    }
}