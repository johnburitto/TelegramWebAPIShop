using AutoMapper;
using Core.Dtos.Create;
using Core.Dtos.Update;
using Core.Entities;

namespace WebAPI.Profiles
{
    public class DiscountProfile : Profile
    {
        public DiscountProfile() 
        {
            CreateMap<DiscountCreateDto, Discount>()
                .ForMember(dest => dest.Products, options => options.MapFrom(src => new List<Product>()))
                .ForMember(dest => dest.ChangedBy, options => options.MapFrom(src => Environment.UserName))
                .ForMember(dest => dest.CreatedAt, options => options.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, options => options.MapFrom(src => DateTime.UtcNow));
            CreateMap<DiscountUpdateDto, Discount>()
                .ForMember(dest => dest.Products, options => options.MapFrom(src => new List<Product>()))
                .ForMember(dest => dest.ChangedBy, options => options.MapFrom(src => Environment.UserName))
                .ForMember(dest => dest.UpdatedAt, options => options.MapFrom(src => DateTime.UtcNow));
        }
    }
}
