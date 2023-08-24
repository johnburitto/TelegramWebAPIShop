using AutoMapper;
using Core.Dtos.Create;
using Core.Dtos.Update;
using Core.Entities;

namespace WebAPI.Profiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<ProductCreateDto, Product>()
                .ForMember(dest => dest.ChangedBy, options => options.MapFrom(src => Environment.UserName))
                .ForMember(dest => dest.CreatedAt, options => options.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, options => options.MapFrom(src => DateTime.UtcNow));
            CreateMap<ProductUpdateDto, Product>()
                .ForMember(dest => dest.ChangedBy, options => options.MapFrom(src => Environment.UserName))
                .ForMember(dest => dest.UpdatedAt, options => options.MapFrom(src => DateTime.UtcNow));
        }
    }
}
