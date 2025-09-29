using AutoMapper;
using MarketplaceDAL.Models;
using OrderManagementBLL.DTOs.OrderItem;

namespace OrderManagementBLL.MappingProfiles;

public class OrderItemMappingProfile  : Profile
{
    public OrderItemMappingProfile()
    {
        CreateMap<OrderItem, OrderItemDto>();
        CreateMap<OrderItemCreateDto, OrderItem>()
            .ForMember(dest => dest.OrderItemId, opt => opt.Ignore())
            .ForMember(dest => dest.OrderId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.RowVer, opt => opt.Ignore());
        CreateMap<OrderItemUpdateDto, OrderItem>()
            .ForMember(dest => dest.OrderId, opt => opt.Ignore())
            .ForMember(dest => dest.ProductId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
    }
}