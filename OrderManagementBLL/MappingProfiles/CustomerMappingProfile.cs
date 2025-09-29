using AutoMapper;
using MarketplaceDAL.Models;
using OrderManagementBLL.DTOs.Customer;

namespace OrderManagementBLL.MappingProfiles;

public class CustomerMappingProfile :  Profile
{
    public CustomerMappingProfile()
    {
        CreateMap<Customer, CustomerDto>();
        CreateMap<CustomerCreateDto, Customer>()
            .ForMember(dest => dest.CustomerId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.RowVer, opt => opt.Ignore())
            .ForMember(dest => dest.Orders, opt => opt.Ignore());
        CreateMap<CustomerUpdateDto, Customer>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.Orders, opt => opt.Ignore());

    }
}