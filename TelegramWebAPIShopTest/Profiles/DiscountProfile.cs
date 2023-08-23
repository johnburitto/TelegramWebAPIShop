using AutoMapper;
using Core.Dtos.Create;
using Core.Dtos.Update;
using Core.Entities;

namespace TelegramWebAPIShopTest.Profiles
{
    public class DiscountProfile : Profile
    {
        public DiscountProfile() 
        {
            CreateMap<DiscountCreateDto, Discount>()
                .ForMember(dest => dest.Products, options => options.MapFrom(src => new List<Product>()));
            CreateMap<DiscountUpdateDto, Discount>()
                .ForMember(dest => dest.Products, options => options.MapFrom(src => new List<Product>()));
        }
    }
}
