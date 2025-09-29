using AutoMapper;
using MarketplaceDAL.Models;
using OrderManagementBLL.DTOs.Payment;

namespace OrderManagementBLL.MappingProfiles;

public class PaymentMappingProfile :  Profile
{
    public PaymentMappingProfile()
    {
        CreateMap<Payment, PaymentDto>();
        CreateMap<PaymentCreateDto, Payment>()
            .ForMember(dest => dest.PaymentId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.RowVer, opt => opt.Ignore());
        CreateMap<PaymentUpdateDto, Payment>()
            .ForMember(dest => dest.OrderId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
    }
    
}