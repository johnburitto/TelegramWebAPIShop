using AutoMapper;
using Core.Dtos.Create;
using Core.Dtos.Update;
using Core.Entities;

namespace TelegramWebAPIShopTest.Profiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<OrderCreateDto, Order>()
                .ForMember(dest => dest.Products, options => options.MapFrom(src => new List<Product>()));
            CreateMap<OrderUpdateDto, Order>()
                .ForMember(dest => dest.Products, options => options.MapFrom(src => new List<Product>()));
        }
    }
}
